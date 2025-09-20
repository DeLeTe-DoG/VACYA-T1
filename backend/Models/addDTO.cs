namespace backend.Models
{
    public class AddSiteDTO
    {
        public string Name { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
        public string ExpectedContent { get; set; } = string.Empty;
    }

    public class AddScenarioDTO
    {
        public string Name { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = "GET";
        public string? Body { get; set; }
        public Dictionary<string, string>? Headers { get; set; }
        public string? ExpectedContent { get; set; }
        public bool CheckJson { get; set; } = false;
        public bool CheckXml { get; set; } = false;
    }
}
