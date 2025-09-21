using System.Net.Http;
using backend.Interfaces;

public class WebhookNotificationChannel : INotificationChannel
{
    private readonly HttpClient _httpClient;
    private readonly string _webhookUrl;

    public WebhookNotificationChannel(HttpClient httpClient, string webhookUrl)
    {
        _httpClient = httpClient;
        _webhookUrl = webhookUrl;
    }

    public async Task SendAsync(string userName, string siteName, string message, NotificationLevel level)
    {
        var payload = new
        {
            User = userName,
            Site = siteName,
            Level = level.ToString(),
            Message = message,
            Time = DateTime.UtcNow
        };

        await _httpClient.PostAsJsonAsync(_webhookUrl, payload);
    }
}
