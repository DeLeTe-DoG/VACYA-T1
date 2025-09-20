using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Routing;

public class WebsiteService
{
    private readonly UserService _userService;
    private readonly List<WebSiteDTO> _websites;
    private readonly IHttpClientFactory _httpClientFactory;

    public WebsiteService(UserService userService, IHttpClientFactory httpClientFactory)
    {
        _userService = userService;
        _websites = new List<WebSiteDTO>();
        _httpClientFactory = httpClientFactory;
    }

    public List<WebSiteDTO> GetAll()
    {
        return _websites;
    }

    public (bool Success, string Message, WebSiteDTO?) Add(
        string url, string userName, string name, string expectedContent)
    {
        var user = _userService.GetByName(userName);

        if (user.Sites.Any(s => s.URL == url))
            return (false, $"Сайт с URL '{url}' уже существует", null);

        if (user.Sites.Any(s => s.Name == name))
            return (false, $"Сайт с именем '{name}' уже существует", null);
        if (user == null)
            return (false, "Пользователь не найден", null);

        var site = new WebSiteDTO
        {
            Id = user.Sites.Count + 1,
            Name = name,
            URL = url,
            ExpectedContent = expectedContent,
            TotalErrors = 0,
            WebSiteData = new(),
            TestsData = new()
        };

        user.Sites.Add(site);
        return (true, "Сайт успешно добавлен", site);
    }

    public (bool Success, string Message, TestScenarioDTO?) AddScenario(
        string userName, string name, bool checkXml, string httpMethod, string url, 
        string? body, Dictionary<string, string>? headers, string? expectedContent, bool checkJson
    )
    {
        var user = _userService.GetByName(userName);
        if (user == null)
            return (false, "Пользователь не найден", null);
    
        // Ищем сайт, к которому добавляем сценарий
        var site = user.Sites.FirstOrDefault(s => s.URL == url);
        if (site == null)
            return (false, $"Сайт с URL '{url}' не найден у пользователя", null);
    
        if (site.TestScenarios.Any(s => s.Name == name))
            return (false, $"Тестовый сценарий с именем '{name}' уже существует", null);
    
        var scenario = new TestScenarioDTO
        {
            Name = name,
            Url = url,
            HttpMethod = httpMethod,
            Body = body,
            Headers = headers ?? new(),
            ExpectedContent = expectedContent,
            CheckJson = checkJson,
            CheckXml = checkXml
        };
    
        site.TestScenarios.Add(scenario);
    
        return (true, "Тестовый сценарий успешно добавлен", scenario);
    }

}
