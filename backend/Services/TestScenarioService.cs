using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using backend.Models;
using backend.Data;
using backend.Entities;
using Microsoft.EntityFrameworkCore;

public class TestScenarioService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AppDbContext _db;

    public TestScenarioService(IHttpClientFactory httpClientFactory, AppDbContext db)
    {
        _httpClientFactory = httpClientFactory;
        _db = db;
    }

    public async Task<ScenarioResultDTO> RunScenarioAsync(int scenarioId)
    {
        // Получаем сценарий из базы
        var scenario = await _db.TestScenarios
            .Include(s => s.WebSite)
            .FirstOrDefaultAsync(s => s.Id == scenarioId);

        if (scenario == null)
            return new ScenarioResultDTO
            {
                Id = $"NOTFOUND/{Guid.NewGuid()}",
                Name = "Сценарий не найден",
                LastChecked = DateTime.UtcNow,
                StatusCode = 0,
                ErrorMessage = "Сценарий не найден"
            };

        var client = _httpClientFactory.CreateClient();

        var result = new ScenarioResultDTO
        {
            LastChecked = DateTime.UtcNow,
            Name = scenario.Name
        };

        try
        {
            // Формируем полный URL: базовый URL сайта + путь API
            var fullUrl = new Uri(new Uri(scenario.WebSite.URL), scenario.ApiPath);

            using var request = new HttpRequestMessage(new HttpMethod(scenario.HttpMethod), fullUrl);

            // Добавляем заголовки, кроме Content-Type
            if (!string.IsNullOrEmpty(scenario.HeadersJson))
            {
                var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(scenario.HeadersJson);
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        if (header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                            continue;
                        request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
            }

            // Если есть тело запроса
            if (!string.IsNullOrEmpty(scenario.Body))
            {
                var content = new StringContent(scenario.Body, Encoding.UTF8);

                if (!string.IsNullOrEmpty(scenario.HeadersJson))
                {
                    var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(scenario.HeadersJson);
                    if (headers != null && headers.TryGetValue("Content-Type", out var contentType))
                    {
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                    }
                    else
                    {
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    }
                }
                else
                {
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                }

                request.Content = content;
            }

            // Отправляем запрос и замеряем время
            var stopwatch = Stopwatch.StartNew();
            var response = await client.SendAsync(request);
            stopwatch.Stop();

            result.StatusCode = (int)response.StatusCode;
            result.ResponseTime = $"{stopwatch.ElapsedMilliseconds} ms";
            result.Id = $"{result.StatusCode}/TEST/{Guid.NewGuid()}";

            var contentString = await response.Content.ReadAsStringAsync();

            // Проверка ожидаемого контента
            if (!string.IsNullOrWhiteSpace(scenario.ExpectedContent) &&
                !contentString.Contains(scenario.ExpectedContent, StringComparison.OrdinalIgnoreCase))
            {
                result.ErrorMessage = "Ожидаемый контент не найден";
            }

            // Проверка JSON
            if (scenario.CheckJson)
            {
                try
                {
                    JsonDocument.Parse(contentString);
                }
                catch
                {
                    result.ErrorMessage = "Некорректный JSON";
                }
            }

            // Проверка XML
            if (scenario.CheckXml)
            {
                try
                {
                    XDocument.Parse(contentString);
                }
                catch
                {
                    result.ErrorMessage = "Некорректный XML";
                }
            }
        }
        catch (Exception ex)
        {
            result.StatusCode = 0;
            result.Id = $"EXCEPTION/{Guid.NewGuid()}";
            result.ErrorMessage = ex.Message;
        }

        return result;
    }
}
