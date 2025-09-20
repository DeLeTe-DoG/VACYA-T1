namespace backend.Models;

public class TestScenarioDTO
{
  public string Url { get; set; } = string.Empty;         
  public string Name { get; set; } = string.Empty;      
  public string HttpMethod { get; set; } = "GET";    
  public Dictionary<string, string>? Headers { get; set; } 
  public string? Body { get; set; }                  
  public string? ExpectedContent { get; set; }        
  public bool CheckJson { get; set; } = false;         
  public bool CheckXml { get; set; } = false;           
}