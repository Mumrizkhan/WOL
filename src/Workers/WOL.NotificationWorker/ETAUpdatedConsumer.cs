using MassTransit;
using Microsoft.Extensions.Logging;
using WOL.NotificationWorker.Services;
using WOL.Shared.Messages.Events;

namespace WOL.NotificationWorker;

public class ETAUpdatedConsumer : IConsumer<ETAUpdatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<ETAUpdatedConsumer> _logger;

    public ETAUpdatedConsumer(
        INotificationService notificationService,
        ILogger<ETAUpdatedConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ETAUpdatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing ETA update notification for Booking {BookingId}", message.BookingId);

        try
        {
            var notificationMessage = $"Your driver is {message.RemainingDistanceKm:F1} km away. " +
                                    $"Estimated arrival: {message.EstimatedArrival:HH:mm}. " +
                                    $"Remaining time: {message.RemainingTime.Hours}h {message.RemainingTime.Minutes}m.";

            await _notificationService.SendPushNotificationAsync(
                message.CustomerId,
                "Driver ETA Update",
                notificationMessage);

            _logger.LogInformation("ETA update notification sent successfully for Booking {BookingId}", message.BookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending ETA update notification for Booking {BookingId}", message.BookingId);
            throw;
        }
    }
}
