using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace backend.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";

        public List<WebSite> Sites { get; set; } = new();
    }

    public class WebSite
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string URL { get; set; } = "";
        public string ExpectedContent { get; set; } = "";
        public string ResponseTime { get; set; } = "";
        public bool IsAvailable { get; set; }
        public string DNS { get; set; } = "";
        public string SSL { get; set; } = "";
        public int TotalErrors { get; set; } = 0;

        public int UserId { get; set; }
        public User User { get; set; }

        public List<WebSiteData> WebSiteData { get; set; } = new();
        public List<TestScenario> TestScenarios { get; set; } = new();
        public List<ScenarioResult> TestsData { get; set; } = new();
    }

    public class WebSiteData
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int? StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime LastChecked { get; set; }

        public int WebSiteId { get; set; }
        public WebSite WebSite { get; set; }
    }
    
    public class TestScenario
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Url { get; set; } = "";       // полный URL для запроса (генерируется сервером)
        public string ApiPath { get; set; } = "";   // путь, который вводит пользователь
        public string HttpMethod { get; set; } = "GET";
        public string? Body { get; set; }
        public string? ExpectedContent { get; set; }
        public bool CheckJson { get; set; } = false;
        public bool CheckXml { get; set; } = false;
        public string? HeadersJson { get; set; }
    
        public int WebSiteId { get; set; }
        public WebSite WebSite { get; set; }
    }


    public class ScenarioResult
    {
      [Key]
      public string Id { get; set; } = Guid.NewGuid().ToString();
      public string Name { get; set; } = string.Empty;
      public int StatusCode { get; set; }
      public string ResponseTime { get; set; } = string.Empty;
      public string ErrorMessage { get; set; } = string.Empty;
      public DateTime LastChecked { get; set; } = DateTime.UtcNow;
      public int WebSiteId { get; set; }  // связь с сайтом
      public WebSite WebSite { get; set; }  // навигационное свойство
    
    }
}
