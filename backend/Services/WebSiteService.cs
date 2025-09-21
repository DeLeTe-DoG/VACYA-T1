using System.Text.Json;
using backend.Data;
using backend.Entities;
using backend.Models;
using Microsoft.EntityFrameworkCore;

public class WebsiteService
{
    private readonly AppDbContext _db;

    public WebsiteService(AppDbContext db)
    {
        _db = db;
    }

    // Получить все сайты пользователя
    public async Task<List<WebSiteDTO>> GetAllAsync(string userName)
    {
        var user = await _db.Users
            .Include(u => u.Sites)
                .ThenInclude(s => s.TestScenarios)
            .Include(u => u.Sites)
                .ThenInclude(s => s.WebSiteData)
            .Include(u => u.Sites)
                .ThenInclude(s => s.TestsData)
            .FirstOrDefaultAsync(u => u.Name == userName);

        if (user == null) return new List<WebSiteDTO>();

        return user.Sites.Select(s => new WebSiteDTO
        {
            Id = s.Id,
            Name = s.Name,
            URL = s.URL,
            ExpectedContent = s.ExpectedContent,
            TotalErrors = s.WebSiteData.Count(d => !string.IsNullOrEmpty(d.ErrorMessage)),
            WebSiteData = s.WebSiteData.Select(d => new WebSiteDataDTO
            {
                Id = d.Id,
                LastChecked = d.LastChecked,
                StatusCode = d.StatusCode,
                ErrorMessage = d.ErrorMessage
            }).ToList(),
            TestsData = s.TestsData.Select(t => new ScenarioResultDTO
            {
                Id = t.Id,
                Name = t.Name,
                StatusCode = t.StatusCode,
                ResponseTime = t.ResponseTime,
                ErrorMessage = t.ErrorMessage,
                LastChecked = t.LastChecked
            }).ToList(),
            TestScenarios = s.TestScenarios.Select(t => new TestScenarioDTO
            {
                Name = t.Name,
                HttpMethod = t.HttpMethod,
                Body = t.Body,
                Headers = string.IsNullOrEmpty(t.HeadersJson)
                    ? new Dictionary<string, string>()
                    : JsonSerializer.Deserialize<Dictionary<string, string>>(t.HeadersJson),
                ExpectedContent = t.ExpectedContent,
                CheckJson = t.CheckJson,
                CheckXml = t.CheckXml
            }).ToList()
        }).ToList();
    }

    // Добавить сайт
    public async Task<(bool Success, string Message, WebSiteDTO?)> AddAsync(string url, string userName, string name, string expectedContent)
    {
        var user = await _db.Users
            .Include(u => u.Sites)
            .FirstOrDefaultAsync(u => u.Name == userName);

        if (user == null) return (false, "Пользователь не найден", null);
        if (user.Sites.Any(s => s.URL == url)) return (false, $"Сайт с URL '{url}' уже существует", null);
        if (user.Sites.Any(s => s.Name == name)) return (false, $"Сайт с именем '{name}' уже существует", null);

        var site = new WebSite
        {
            Name = name,
            URL = url,
            ExpectedContent = expectedContent,
            WebSiteData = new List<WebSiteData>(),
            TestsData = new List<ScenarioResult>(),
            TestScenarios = new List<TestScenario>()
        };

        user.Sites.Add(site);
        await _db.SaveChangesAsync();

        var siteDto = new WebSiteDTO
        {
            Id = site.Id,
            Name = site.Name,
            URL = site.URL,
            ExpectedContent = site.ExpectedContent,
            TotalErrors = 0,
            WebSiteData = new List<WebSiteDataDTO>(),
            TestsData = new List<ScenarioResultDTO>(),
            TestScenarios = new List<TestScenarioDTO>()
        };

        return (true, "Сайт успешно добавлен", siteDto);
    }

    // Удаление сайта пользователя
    public async Task DeleteSiteAsync(string userName, int siteId)
    {
        var user = await _db.Users
            .Include(u => u.Sites)
                .ThenInclude(s => s.TestScenarios)
            .Include(u => u.Sites)
                .ThenInclude(s => s.WebSiteData)
            .Include(u => u.Sites)
                .ThenInclude(s => s.TestsData)
            .FirstOrDefaultAsync(u => u.Name == userName);

        if (user == null) throw new Exception("Пользователь не найден");

        var site = user.Sites.FirstOrDefault(s => s.Id == siteId);
        if (site == null) throw new Exception("Сайт не найден");

        _db.WebSites.Remove(site);
        await _db.SaveChangesAsync();
    }

    public async Task<(bool Success, string Message)> DeleteScenarioAsync(string userName, string siteName, string scenarioName)
    {
        var user = await _db.Users
            .Include(u => u.Sites)
            .ThenInclude(s => s.TestScenarios)
            .FirstOrDefaultAsync(u => u.Name == userName);

        if (user == null)
            return (false, "Пользователь не найден");

        var site = user.Sites.FirstOrDefault(s => s.Name == siteName);
        if (site == null)
            return (false, "Сайт не найден");

        var scenario = site.TestScenarios.FirstOrDefault(sc => sc.Name == scenarioName);
        if (scenario == null)
            return (false, "Сценарий не найден");

        _db.TestScenarios.Remove(scenario);
        await _db.SaveChangesAsync();

        return (true, "Сценарий удалён");
    }

    public async Task<(bool Success, string Message, TestScenarioDTO?)> AddScenarioAsync(
        string userName,
        string siteName,
        string name,
        string httpMethod,
        string? body,
        Dictionary<string, string>? headers,
        string? expectedContent,
        bool checkJson,
        bool checkXml,
        string apiPath   // <- получаем от пользователя
    )
    {
        // Ищем пользователя
        var user = await _db.Users
            .Include(u => u.Sites)
            .FirstOrDefaultAsync(u => u.Name == userName);

        if (user == null) return (false, "Пользователь не найден", null);

        // Ищем сайт пользователя
        var site = user.Sites.FirstOrDefault(s => s.Name == siteName);
        if (site == null) return (false, "Сайт не найден", null);

        // Сериализация Headers в JSON
        var headersJson = headers != null
            ? JsonSerializer.Serialize(headers)
            : null;

        var scenario = new TestScenario
        {
            Name = name,
            ApiPath = apiPath,          // <- сохраняем путь в базу
            HttpMethod = httpMethod,
            Body = body,
            HeadersJson = headersJson,
            ExpectedContent = expectedContent,
            CheckJson = checkJson,
            CheckXml = checkXml,
            WebSiteId = site.Id
        };

        _db.TestScenarios.Add(scenario);
        await _db.SaveChangesAsync();

        // Маппинг обратно в DTO
        var scenarioDto = new TestScenarioDTO
        {
            Name = scenario.Name,
            ApiPath = scenario.ApiPath,   // <- возвращаем пользователю
            HttpMethod = scenario.HttpMethod,
            Body = scenario.Body,
            Headers = !string.IsNullOrEmpty(scenario.HeadersJson)
                ? JsonSerializer.Deserialize<Dictionary<string, string>>(scenario.HeadersJson)
                : new Dictionary<string, string>(),
            ExpectedContent = scenario.ExpectedContent,
            CheckJson = scenario.CheckJson,
            CheckXml = scenario.CheckXml
        };

        return (true, "Сценарий добавлен", scenarioDto);
    }


    public async Task<List<WebSiteDTO>> FilterByDateAsync(int userId, DateTime dateFrom, DateTime dateTo)
    {
        // приводим к UTC
        var dateFromStart = DateTime.SpecifyKind(dateFrom.Date, DateTimeKind.Utc);
        var dateToEnd = DateTime.SpecifyKind(dateTo.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

        var sites = await _db.WebSites
            .Where(s => s.UserId == userId)
            .Include(s => s.WebSiteData
                .Where(d => d.LastChecked >= dateFromStart && d.LastChecked <= dateToEnd))
            .ToListAsync();

        var result = sites
            .Where(s => s.WebSiteData.Any())
            .Select(s => new WebSiteDTO
            {
                Id = s.Id,
                Name = s.Name,
                URL = s.URL,
                TotalErrors = s.WebSiteData.Count(d => !string.IsNullOrEmpty(d.ErrorMessage)),
                WebSiteData = s.WebSiteData.Select(d => new WebSiteDataDTO
                {
                    Id = d.Id,
                    StatusCode = d.StatusCode,
                    ErrorMessage = d.ErrorMessage,
                    LastChecked = d.LastChecked
                }).ToList()
            })
            .ToList();

        return result;
    }
}
