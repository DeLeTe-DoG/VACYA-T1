using System;

namespace backend.Models;

public class WebSiteDataDTO
{
    public string Id { get; set; }
    // public bool IsAvailable { get; set; }
    public int? StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime LastChecked { get; set; }

}