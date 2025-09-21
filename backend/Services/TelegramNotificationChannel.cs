using System.Net.Http;
using System.Text.Json;
using backend.Interfaces;

public class TelegramNotificationChannel : INotificationChannel
{
    private readonly HttpClient _httpClient;
    private readonly string _botToken;
    private readonly string _chatId;

    public TelegramNotificationChannel(HttpClient httpClient, string botToken, string chatId)
    {
        _httpClient = httpClient;
        _botToken = botToken;
        _chatId = chatId;
    }

    public async Task SendAsync(string userName, string siteName, string message, NotificationLevel level)
    {
        var text = $"[{level}] Пользователь: {userName}, Сайт: {siteName}\n{message}";
        var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";

        var payload = new
        {
            chat_id = _chatId,
            text = text
        };

        await _httpClient.PostAsJsonAsync(url, payload);
    }
}
