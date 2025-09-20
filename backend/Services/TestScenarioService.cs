using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using backend.Models;

public class TestScenarioService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public TestScenarioService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ScenarioResultDTO> RunScenarioAsync(TestScenarioDTO scenario)
    {
        var client = _httpClientFactory.CreateClient();
        var result = new ScenarioResultDTO
        {
            LastChecked = DateTime.UtcNow,
            Name = scenario.Name
        };

        try
        {
            // Создаем HTTP-запрос
            using var request = new HttpRequestMessage(new HttpMethod(scenario.HttpMethod), scenario.Url);

            // Добавляем заголовки (кроме Content-Type)
            if (scenario.Headers != null)
            {
                foreach (var header in scenario.Headers)
                {
                    if (header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                        continue; // Content-Type добавим ниже, если есть тело

                    // TryAddWithoutValidation — чтобы не падало на "нестандартных" заголовках
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            // Если есть тело запроса — добавляем его и Content-Type
            if (!string.IsNullOrEmpty(scenario.Body))
            {
                var content = new StringContent(scenario.Body, Encoding.UTF8);

                // Устанавливаем Content-Type, если указан явно в Headers
                if (scenario.Headers != null && 
                    scenario.Headers.TryGetValue("Content-Type", out var contentType))
                {
                    content.Headers.ContentType = 
                        new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                }
                else
                {
                    content.Headers.ContentType = 
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                }

                request.Content = content;
            }

            // Засекаем время отклика
            var stopwatch = Stopwatch.StartNew();
            var response = await client.SendAsync(request);
            stopwatch.Stop();

            result.StatusCode = (int)response.StatusCode;
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
                try { JsonDocument.Parse(contentString); }
                catch { result.ErrorMessage = "Некорректный JSON"; }
            }

            // Проверка XML
            if (scenario.CheckXml)
            {
                try { XDocument.Parse(contentString); }
                catch { result.ErrorMessage = "Некорректный XML"; }
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