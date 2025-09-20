using System.Net.Http;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using backend.Models;
using backend.Services;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text;

public class MonitoringBackgroundService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly UserService _userService;
    private readonly TestScenarioService _testService;

    public MonitoringBackgroundService(IHttpClientFactory httpClientFactory, UserService userService, TestScenarioService testScenario)
    {
        _httpClientFactory = httpClientFactory;
        _userService = userService;
        _testService = testScenario;
    }

    private bool checkDNS(string host)
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

            var cert = new X509Certificate2(sslStream.RemoteCertificate);
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
            var users = _userService.GetAll();

            foreach (var user in users)
            {
                foreach (var site in user.Sites.ToList())
                {
                    var data = new WebSiteDataDTO
                    {
                        LastChecked = DateTime.UtcNow
                    };
                    // 1. Проверка корректности URL
                    if (!Uri.TryCreate(site.URL, UriKind.Absolute, out var uri))
                    {
                        data.StatusCode = 0;
                        data.ErrorMessage = "Некорректный URL";
                        data.Id = $"INVALID_URL/BACKGROUND/{Guid.NewGuid()}";
                        site.IsAvailable = false;
                        site.WebSiteData = site.WebSiteData.Append(data).ToList();
                        site.TotalErrors = site.WebSiteData.Count(e => !string.IsNullOrWhiteSpace(e.ErrorMessage));
                        continue;
                    }

                    try
                    {
                        // 2. Проверка DNS
                        bool dnsOK = checkDNS(uri.Host);
                        if (!dnsOK)
                        {
                            data.StatusCode = 0;
                            data.ErrorMessage = "DNS не найден";
                            data.Id = $"DNS_ERROR/BACKGROUND/{Guid.NewGuid()}";
                            site.IsAvailable = false;
                            site.DNS = "DNS не найден";

                            site.WebSiteData = site.WebSiteData.Append(data).ToList();
                            site.TotalErrors = site.WebSiteData.Count(e => !string.IsNullOrWhiteSpace(e.ErrorMessage));
                            continue;
                        }
                        site.DNS = "OK";

                        // 3. Проверка SSL (если HTTPS)
                        bool sslOk = uri.Scheme == Uri.UriSchemeHttps ? CheckSslCertificate(uri.Host) : true;
                        if (!sslOk)
                        {
                            data.StatusCode = 0;
                            data.ErrorMessage = "SSL сертификат недействителен";
                            data.Id = $"SSL_ERROR/BACKGROUND/{Guid.NewGuid()}";
                            site.IsAvailable = false;
                            site.SSL = "Сертификат недействителен";

                            site.WebSiteData = site.WebSiteData.Append(data).ToList();
                            site.TotalErrors = site.WebSiteData.Count(e => !string.IsNullOrWhiteSpace(e.ErrorMessage));
                            continue;
                        }
                        site.SSL = "OK";

                        // 4. HTTP-запрос с замером времени отклика
                        var stopwatch = Stopwatch.StartNew();
                        var response = await client.GetAsync(uri, stoppingToken);
                        stopwatch.Stop();
                        site.ResponseTime = $"{stopwatch.ElapsedMilliseconds} ms";

                        int status = (int)response.StatusCode;
                        data.StatusCode = status;
                        data.Id = $"{status}/BACKGROUND/{Guid.NewGuid()}";

                        // Проверка статуса HTTP
                        if (status >= 200 && status < 400)
                        {
                            site.IsAvailable = true;
                        }
                        else 
                        {
                            site.IsAvailable = false;
                            data.ErrorMessage = response.ReasonPhrase ?? $"HTTP Error {status}";
                        }

                        // 5. Проверка содержимого (если задано expectedContent)
                        if (!string.IsNullOrWhiteSpace(site.ExpectedContent))
                        {
                            try
                            {
                                var bytes = await response.Content.ReadAsByteArrayAsync(stoppingToken);
                                var content = Encoding.UTF8.GetString(bytes);

                                if (!content.Contains(site.ExpectedContent, StringComparison.OrdinalIgnoreCase))
                                {
                                    data.ErrorMessage = "Ожидаемый контент не найден";
                                    data.Id = $"CONTENT_ERROR/BACKGROUND/{Guid.NewGuid()}";
                                    site.IsAvailable = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                data.ErrorMessage = $"Ошибка при проверке контента: {ex.Message}";
                                data.Id = $"CONTENT_EXCEPTION/BACKGROUND/{Guid.NewGuid()}";
                                site.IsAvailable = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        data.StatusCode = 0;
                        data.ErrorMessage = ex.Message;
                        data.Id = $"EXCEPTION/{Guid.NewGuid()}";
                        site.IsAvailable = false;
                    }

                    foreach (var scenario in site.TestScenarios)
                    {
                        if (scenario == null)
                        {
                            continue;
                        }
                        var result = await _testService.RunScenarioAsync(scenario);
                        if (result.ErrorMessage == "" || result.ErrorMessage == null)
                        {
                            continue;
                        }
                        site.TestsData.Add(result);
                        if (!string.IsNullOrEmpty(result.ErrorMessage)) site.TotalErrors++;
                    }

                    site.WebSiteData = site.WebSiteData.Append(data).ToList();
                    site.TotalErrors = site.WebSiteData.Count(e => !string.IsNullOrWhiteSpace(e.ErrorMessage));

                    await Task.Delay(500, stoppingToken);
                }
            }
            await Task.Delay(5000, stoppingToken);
        }
    }
}