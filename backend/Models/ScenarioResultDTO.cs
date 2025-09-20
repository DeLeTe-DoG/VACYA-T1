namespace backend.Models;

public class ScenarioResultDTO
{
  public string Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public int StatusCode { get; set; }
  public string ResponseTime { get; set; } = string.Empty;
  public string ErrorMessage { get; set; } = string.Empty;
  public DateTime LastChecked { get; set; }
}