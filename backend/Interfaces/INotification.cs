namespace backend.Interfaces;

public interface INotificationSender
{
  Task SendAsync(string recipient, string subject, string message);
}
public interface INotificationChannel
{
  Task SendAsync(string userName, string siteName, string message, NotificationLevel level);
}

public interface INotificationService
{
    Task NotifyAsync(string userName, string siteName, string message, NotificationLevel level);
}

public enum NotificationLevel
{
  Info,
  Warning,
  Critical
}