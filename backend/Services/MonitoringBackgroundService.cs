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

            try
            {
                // Пакетная загрузка пользователей
                const int batchSize = 50;
                int skip = 0;
                List<User> usersBatch;

                do
                {
                    usersBatch = await _db.Users
                        .Include(u => u.Sites)
                            .ThenInclude(s => s.TestScenarios)
                        .Include(u => u.Sites)
                            .ThenInclude(s => s.WebSiteData)
                        .Include(u => u.Sites)
                            .ThenInclude(s => s.TestsData)
                        .AsNoTracking()
                        .Skip(skip)
                        .Take(batchSize)
                        .ToListAsync(stoppingToken);

                    skip += batchSize;

                    foreach (var user in usersBatch)
                    {
                        foreach (var site in user.Sites)
                        {
                            var dbSite = await _db.WebSites
                                .Include(s => s.WebSiteData)
                                .Include(s => s.TestsData)
                                .Include(s => s.TestScenarios)
                                .FirstOrDefaultAsync(s => s.Id == site.Id, stoppingToken);

                            if (dbSite == null) continue;

                            var data = new WebSiteData
                            {
                                Id = Guid.NewGuid().ToString(),
                                LastChecked = DateTime.UtcNow,
                                WebSiteId = dbSite.Id
                            };

                            try
                            {
                                if (!Uri.TryCreate(dbSite.URL, UriKind.Absolute, out var uri))
                                    throw new Exception("Некорректный URL");

                                // DNS
                                if (!CheckDNS(uri.Host)) throw new Exception("DNS не найден");
                                dbSite.DNS = "OK";

                                // SSL
                                if (uri.Scheme == Uri.UriSchemeHttps && !CheckSslCertificate(uri.Host))
                                    throw new Exception("SSL сертификат недействителен");
                                dbSite.SSL = "OK";

                                // HTTP запрос
                                var stopwatch = Stopwatch.StartNew();
                                var response = await client.GetAsync(uri, stoppingToken);
                                stopwatch.Stop();

                                dbSite.ResponseTime = $"{stopwatch.ElapsedMilliseconds} ms";
                                data.StatusCode = (int)response.StatusCode;

                                if (!response.IsSuccessStatusCode)
                                    data.ErrorMessage = response.ReasonPhrase ?? $"HTTP Error {data.StatusCode}";

                                // Проверка контента
                                if (!string.IsNullOrWhiteSpace(dbSite.ExpectedContent))
                                {
                                    var content = await response.Content.ReadAsStringAsync(stoppingToken);
                                    if (!content.Contains(dbSite.ExpectedContent, StringComparison.OrdinalIgnoreCase))
                                    {
                                        data.ErrorMessage = "Ожидаемый контент не найден";
                                        dbSite.IsAvailable = false;
                                    }
                                }

                                dbSite.IsAvailable = string.IsNullOrWhiteSpace(data.ErrorMessage);
                            }
                            catch (Exception ex)
                            {
                                data.StatusCode = 0;
                                data.ErrorMessage = ex.Message;
                                dbSite.IsAvailable = false;
                            }

                            _db.WebSiteData.Add(data);
                            dbSite.TotalErrors = dbSite.WebSiteData.Count(e => !string.IsNullOrWhiteSpace(e.ErrorMessage));
                            await _db.SaveChangesAsync(stoppingToken);

                            // Тестовые сценарии
                            foreach (var scenario in dbSite.TestScenarios ?? Enumerable.Empty<TestScenario>())
                            {
                                try
                                {
                                    var resultDto = await _testService.RunScenarioAsync(scenario.Id);

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
                                catch (Exception ex)
                                {
                                    // Логируем, но продолжаем
                                    Console.WriteLine($"Ошибка теста {scenario.Id}: {ex.Message}");
                                }
                            }

                            await _db.SaveChangesAsync(stoppingToken);
                            await Task.Delay(500, stoppingToken);
                        }
                    }
                } while (usersBatch.Count > 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка BackgroundService: {ex.Message}");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}