using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using backend.Entities;
using backend.Services;
using backend.Data;

public class MonitoringBackgroundService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IServiceScopeFactory _scopeFactory;

    public MonitoringBackgroundService(
        IHttpClientFactory httpClientFactory,
        IServiceScopeFactory scopeFactory)
    {
        _httpClientFactory = httpClientFactory;
        _scopeFactory = scopeFactory;
    }

    private bool CheckDNS(string host)
    {
        try
        {
            var entry = Dns.GetHostEntry(host);
            return entry.AddressList.Length > 0;
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
                foreach (var site in user.Sites)
                {
                    var data = new WebSiteData
                    {
                        Id = Guid.NewGuid().ToString(),
                        LastChecked = DateTime.UtcNow,
                        WebSiteId = site.Id
                    };

                    // 1. Проверка корректности URL
                    if (!Uri.TryCreate(site.URL, UriKind.Absolute, out var uri))
                    {
                        data.StatusCode = 0;
                        data.ErrorMessage = "Некорректный URL";
                        site.IsAvailable = false;

                        _db.WebSiteData.Add(data);
                        await _db.SaveChangesAsync(stoppingToken);
                        continue;
                    }

                    try
                    {
                        // 2. Проверка DNS
                        bool dnsOK = CheckDNS(uri.Host);
                        if (!dnsOK)
                        {
                            data.StatusCode = 0;
                            data.ErrorMessage = "DNS не найден";
                            site.IsAvailable = false;
                            site.DNS = "DNS не найден";

                            _db.WebSiteData.Add(data);
                            await _db.SaveChangesAsync(stoppingToken);
                            continue;
                        }
                        site.DNS = "OK";

                        // 3. Проверка SSL
                        bool sslOk = uri.Scheme == Uri.UriSchemeHttps ? CheckSslCertificate(uri.Host) : true;
                        if (!sslOk)
                        {
                            data.StatusCode = 0;
                            data.ErrorMessage = "SSL сертификат недействителен";
                            site.IsAvailable = false;
                            site.SSL = "Сертификат недействителен";

                            _db.WebSiteData.Add(data);
                            await _db.SaveChangesAsync(stoppingToken);
                            continue;
                        }
                        site.SSL = "OK";

                        // 4. HTTP-запрос с замером времени
                        var stopwatch = Stopwatch.StartNew();
                        var response = await client.GetAsync(uri, stoppingToken);
                        stopwatch.Stop();

                        site.ResponseTime = $"{stopwatch.ElapsedMilliseconds} ms";

                        int status = (int)response.StatusCode;
                        data.StatusCode = status;

                        if (status >= 200 && status < 400)
                            site.IsAvailable = true;
                        else
                        {
                            site.IsAvailable = false;
                            data.ErrorMessage = response.ReasonPhrase ?? $"HTTP Error {status}";
                        }

                        // 5. Проверка содержимого (если задан ExpectedContent)
                        if (!string.IsNullOrWhiteSpace(site.ExpectedContent))
                        {
                            try
                            {
                                var bytes = await response.Content.ReadAsByteArrayAsync(stoppingToken);
                                var content = Encoding.UTF8.GetString(bytes);

                                if (!content.Contains(site.ExpectedContent, StringComparison.OrdinalIgnoreCase))
                                {
                                    data.ErrorMessage = "Ожидаемый контент не найден";
                                    site.IsAvailable = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                data.ErrorMessage = $"Ошибка при проверке контента: {ex.Message}";
                                site.IsAvailable = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        data.StatusCode = 0;
                        data.ErrorMessage = ex.Message;
                        site.IsAvailable = false;
                    }

                    // Сохраняем проверку сайта
                    _db.WebSiteData.Add(data);
                    site.TotalErrors = site.WebSiteData.Count(e => !string.IsNullOrWhiteSpace(e.ErrorMessage));
                    await _db.SaveChangesAsync(stoppingToken);

                    // 6. Запуск тестовых сценариев
                    foreach (var scenarioEntity in site.TestScenarios ?? Enumerable.Empty<TestScenario>())
                    {
                        var scenarioDto = new backend.Models.TestScenarioDTO
                        {
                            Name = scenarioEntity.Name,
                            Url = scenarioEntity.Url,
                            HttpMethod = scenarioEntity.HttpMethod,
                            Body = scenarioEntity.Body,
                            ExpectedContent = scenarioEntity.ExpectedContent,
                            CheckJson = scenarioEntity.CheckJson,
                            CheckXml = scenarioEntity.CheckXml
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
                            WebSiteId = site.Id
                        };

                        site.TestsData.Add(resultEntity);
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
