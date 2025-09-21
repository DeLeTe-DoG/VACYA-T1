using backend.Interfaces;
using Microsoft.Extensions.Caching.Memory;

public class NotificationService : INotificationService
{
    private readonly INotificationChannel[] _channels;
    private readonly IMemoryCache _cache;
    private readonly ILogger<NotificationService> _logger;

    // TTL для антиспама (например, 5 минут)
    private static readonly TimeSpan SuppressionWindow = TimeSpan.FromMinutes(5);

    public NotificationService(
        IEnumerable<INotificationChannel> channels,
        IMemoryCache cache,
        ILogger<NotificationService> logger)
    {
        _channels = channels.ToArray();
        _cache = cache;
        _logger = logger;
    }

    public async Task NotifyAsync(string userName, string siteName, string message, NotificationLevel level)
    {
        string key = $"{userName}:{siteName}:{message}:{level}";

        // Проверяем, не отправляли ли недавно такое же уведомление
        if (_cache.TryGetValue(key, out _))
        {
            _logger.LogInformation("Уведомление подавлено (антиспам): {Message}", message);
            return;
        }

        // Сохраняем в кэш, чтобы не слать повторно
        _cache.Set(key, true, SuppressionWindow);

        foreach (var channel in _channels)
        {
            try
            {
                await channel.SendAsync(userName, siteName, message, level);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке через канал {Channel}", channel.GetType().Name);
            }
        }
    }
}
