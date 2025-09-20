using System.Text.Json.Serialization;
namespace backend.Models;
public class LoginDTO
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("password")] 
    public string Password { get; set; }
    
    [JsonPropertyName("email")]
    public string Email { get; set; }
}