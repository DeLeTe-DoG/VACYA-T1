namespace backend.Models;

public class WebSiteDTO
{
    public int Id { get; set; }
    public string URL { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ExpectedContent { get; set; } = string.Empty;
    public string ResponseTime { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public string DNS { get; set; } = string.Empty;
    public string SSL { get; set; } = string.Empty;
    public int TotalErrors { get; set; } = 0;
    public List<WebSiteDataDTO> WebSiteData { get; set; } = [];
    public List<TestScenarioDTO> TestScenarios { get; set; } = [];
    public List<ScenarioResultDTO> TestsData { get; set; } = [];
}