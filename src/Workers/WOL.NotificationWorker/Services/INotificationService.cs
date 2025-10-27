namespace WOL.NotificationWorker.Services;

public interface INotificationService
{
    Task SendPushNotificationAsync(string userId, string title, string message, Dictionary<string, string>? data = null);
    Task SendSmsAsync(string phoneNumber, string message);
    Task SendEmailAsync(string email, string subject, string body);
}
