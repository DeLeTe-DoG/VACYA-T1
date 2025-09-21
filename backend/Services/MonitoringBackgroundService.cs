using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using backend.Entities;
using backend.Data;
using backend.Services;

public class MonitoringBackgroundService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IServiceScopeFactory _scopeFactory;

    public MonitoringBackgroundService(IHttpClientFactory httpClientFactory, IServiceScopeFactory scopeFactory)
    {
        _httpClientFactory = httpClientFactory;
        _scopeFactory = scopeFactory;
    }

    private bool CheckDNS(string host)
    {
        try
        {
            return Dns.GetHostEntry(host).AddressList.Length > 0;
        }
        catch
        {
            return false;
        }
    }

    private bool CheckSslCertificate(string host)
    {
        try
        {
            using var client = new TcpClient();
            client.Connect(host, 443);

            using var sslStream = new SslStream(client.GetStream(), false,
                new RemoteCertificateValidationCallback((sender, cert, chain, errors) => true));

            sslStream.AuthenticateAsClient(host);

            var remoteCert = sslStream.RemoteCertificate;
            if (remoteCert == null) return false;

            var cert = new X509Certificate2(remoteCert);
            return DateTime.UtcNow >= cert.NotBefore && DateTime.UtcNow <= cert.NotAfter;
        }
        catch
        {
            return false;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = _httpClientFactory.CreateClient();

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var _testService = scope.ServiceProvider.GetRequiredService<TestScenarioService>();

            var users = await _db.Users
                .Include(u => u.Sites)
                    .ThenInclude(s => s.TestScenarios)
                .Include(u => u.Sites)
                    .ThenInclude(s => s.WebSiteData)
                .Include(u => u.Sites)
                    .ThenInclude(s => s.TestsData)
                .ToListAsync(stoppingToken);

            foreach (var user in users)
            {
                foreach (var site in user.Sites.ToList())
                {
                    // Получаем сайт напрямую из БД, чтобы EF точно знал его Id
                    var dbSite = await _db.WebSites
                        .Include(s => s.WebSiteData)
                        .Include(s => s.TestsData)
                        .Include(s => s.TestScenarios)
                        .FirstOrDefaultAsync(s => s.Id == site.Id, stoppingToken);

                    if (dbSite == null)
                        continue; // сайт удалён, пропускаем

                    var data = new WebSiteData
                    {
                        Id = Guid.NewGuid().ToString(),
                        LastChecked = DateTime.UtcNow,
                        WebSiteId = dbSite.Id
                    };

                    // 1. Проверка корректности URL
                    if (!Uri.TryCreate(dbSite.URL, UriKind.Absolute, out var uri))
                    {
                        data.StatusCode = 0;
                        data.ErrorMessage = "Некорректный URL";
                        dbSite.IsAvailable = false;

                        _db.WebSiteData.Add(data);
                        await _db.SaveChangesAsync(stoppingToken);
                        continue;
                    }

                    try
                    {
                        // 2. Проверка DNS
                        if (!CheckDNS(uri.Host))
                        {
                            data.StatusCode = 0;
                            data.ErrorMessage = "DNS не найден";
                            dbSite.IsAvailable = false;
                            dbSite.DNS = "DNS не найден";

                            _db.WebSiteData.Add(data);
                            await _db.SaveChangesAsync(stoppingToken);
                            continue;
                        }
                        dbSite.DNS = "OK";

                        // 3. Проверка SSL
                        bool sslOk = uri.Scheme == Uri.UriSchemeHttps ? CheckSslCertificate(uri.Host) : true;
                        if (!sslOk)
                        {
                            data.StatusCode = 0;
                            data.ErrorMessage = "SSL сертификат недействителен";
                            dbSite.IsAvailable = false;
                            dbSite.SSL = "Сертификат недействителен";

                            _db.WebSiteData.Add(data);
                            await _db.SaveChangesAsync(stoppingToken);
                            continue;
                        }
                        dbSite.SSL = "OK";

                        // 4. HTTP-запрос
                        var stopwatch = Stopwatch.StartNew();
                        var response = await client.GetAsync(uri, stoppingToken);
                        stopwatch.Stop();

                        dbSite.ResponseTime = $"{stopwatch.ElapsedMilliseconds} ms";
                        data.StatusCode = (int)response.StatusCode;

                        if (response.IsSuccessStatusCode)
                        {
                            dbSite.IsAvailable = true;
                        }
                        else
                        {
                            dbSite.IsAvailable = false;
                            data.ErrorMessage = response.ReasonPhrase ?? $"HTTP Error {data.StatusCode}";
                        }

                        // 5. Проверка содержимого
                        if (!string.IsNullOrWhiteSpace(dbSite.ExpectedContent))
                        {
                            var bytes = await response.Content.ReadAsByteArrayAsync(stoppingToken);
                            var content = Encoding.UTF8.GetString(bytes);

                            if (!content.Contains(dbSite.ExpectedContent, StringComparison.OrdinalIgnoreCase))
                            {
                                data.ErrorMessage = "Ожидаемый контент не найден";
                                dbSite.IsAvailable = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        data.StatusCode = 0;
                        data.ErrorMessage = ex.Message;
                        dbSite.IsAvailable = false;
                    }

                    // Добавляем данные о проверке сайта
                    _db.WebSiteData.Add(data);
                    dbSite.TotalErrors = dbSite.WebSiteData.Count(e => !string.IsNullOrWhiteSpace(e.ErrorMessage));
                    await _db.SaveChangesAsync(stoppingToken);

                    // 6. Запуск тестовых сценариев
                    foreach (var scenario in dbSite.TestScenarios ?? Enumerable.Empty<TestScenario>())
                    {
                        var scenarioDto = new backend.Models.TestScenarioDTO
                        {
                            Name = scenario.Name,
                            Url = scenario.Url,
                            HttpMethod = scenario.HttpMethod,
                            Body = scenario.Body,
                            ExpectedContent = scenario.ExpectedContent,
                            CheckJson = scenario.CheckJson,
                            CheckXml = scenario.CheckXml
                        };

                        var resultDto = await _testService.RunScenarioAsync(scenarioDto);

                        var resultEntity = new ScenarioResult
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = resultDto.Name,
                            StatusCode = resultDto.StatusCode,
                            ResponseTime = resultDto.ResponseTime,
                            ErrorMessage = resultDto.ErrorMessage,
                            LastChecked = DateTime.UtcNow,
                            WebSiteId = dbSite.Id
                        };

                        dbSite.TestsData.Add(resultEntity);
                        _db.ScenarioResults.Add(resultEntity);
                    }

                    await _db.SaveChangesAsync(stoppingToken);
                    await Task.Delay(500, stoppingToken);
                }
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}
