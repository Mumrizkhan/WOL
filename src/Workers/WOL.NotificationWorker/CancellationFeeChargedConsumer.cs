using MassTransit;
using Microsoft.Extensions.Logging;
using WOL.NotificationWorker.Services;
using WOL.Shared.Messages.Events;

namespace WOL.NotificationWorker;

public class CancellationFeeChargedConsumer : IConsumer<CancellationFeeChargedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<CancellationFeeChargedConsumer> _logger;

    public CancellationFeeChargedConsumer(
        INotificationService notificationService,
        ILogger<CancellationFeeChargedConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CancellationFeeChargedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing CancellationFeeCharged notification for Booking {BookingId}", message.BookingId);

        try
        {
            var notificationMessage = $"A cancellation fee of SAR {message.Amount:F2} has been charged for booking. " +
                                    $"Reason: {message.Reason}. {message.Notes}";

            await _notificationService.SendPushNotificationAsync(
                message.ChargedToUserId,
                "Cancellation Fee Charged",
                notificationMessage);

            await _notificationService.SendSmsAsync(
                message.ChargedToUserId,
                notificationMessage);

            _logger.LogInformation("CancellationFeeCharged notification sent successfully for Booking {BookingId}", message.BookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending CancellationFeeCharged notification for Booking {BookingId}", message.BookingId);
            throw;
        }
    }
}
