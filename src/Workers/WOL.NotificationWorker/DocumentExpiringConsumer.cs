using MassTransit;
using Microsoft.Extensions.Logging;
using WOL.NotificationWorker.Services;
using WOL.Shared.Messages.Events;

namespace WOL.NotificationWorker;

public class DocumentExpiringConsumer : IConsumer<DocumentExpiringEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<DocumentExpiringConsumer> _logger;

    public DocumentExpiringConsumer(
        INotificationService notificationService,
        ILogger<DocumentExpiringConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DocumentExpiringEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing DocumentExpiring notification for Document {DocumentId}", message.DocumentId);

        try
        {
            var notificationMessage = $"Your {message.DocumentType} (Number: {message.DocumentNumber}) " +
                                    $"is expiring in {message.DaysUntilExpiry} days on {message.ExpiryDate:yyyy-MM-dd}. " +
                                    $"Please renew it to avoid service disruption.";

            await _notificationService.SendPushNotificationAsync(
                message.UserId,
                "Document Expiring Soon",
                notificationMessage);

            await _notificationService.SendSmsAsync(
                message.UserId,
                notificationMessage);

            await _notificationService.SendEmailAsync(
                message.UserId,
                "Document Expiring Soon - Action Required",
                notificationMessage);

            _logger.LogInformation("DocumentExpiring notification sent successfully for Document {DocumentId}", message.DocumentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending DocumentExpiring notification for Document {DocumentId}", message.DocumentId);
            throw;
        }
    }
}
