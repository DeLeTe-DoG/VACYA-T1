using backend.Interfaces;

public class ConsoleNotificationChannel : INotificationChannel
{
    public Task SendAsync(string userName, string siteName, string message, NotificationLevel level)
    {
        Console.WriteLine($"[Console][{level}] {userName}/{siteName}: {message}");
        return Task.CompletedTask;
    }
}
