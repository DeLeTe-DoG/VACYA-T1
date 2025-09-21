using System.Net;
using System.Net.Mail;
using backend.Interfaces;

public class EmailNotificationChannel : INotificationChannel
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _from;
    private readonly string _password;

    public EmailNotificationChannel(string smtpHost, int smtpPort, string from, string password)
    {
        _smtpHost = smtpHost;
        _smtpPort = smtpPort;
        _from = from;
        _password = password;
    }

    public async Task SendAsync(string userName, string siteName, string message, NotificationLevel level)
    {
        using var client = new SmtpClient(_smtpHost, _smtpPort)
        {
            Credentials = new NetworkCredential(_from, _password),
            EnableSsl = true
        };

        var mail = new MailMessage(_from, $"{userName}@example.com")
        {
            Subject = $"[{level}] Проблема на сайте {siteName}",
            Body = message
        };

        await client.SendMailAsync(mail);
    }
}
