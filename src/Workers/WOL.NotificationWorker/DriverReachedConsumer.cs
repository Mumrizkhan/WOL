using MassTransit;
using Microsoft.Extensions.Logging;
using WOL.NotificationWorker.Services;
using WOL.Shared.Messages.Events;

namespace WOL.NotificationWorker;

public class DriverReachedConsumer : IConsumer<WOL.Shared.Messages.Events.DriverReachedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<DriverReachedConsumer> _logger;

    public DriverReachedConsumer(
        INotificationService notificationService,
        ILogger<DriverReachedConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<WOL.Shared.Messages.Events.DriverReachedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing DriverReached notification for Booking {BookingNumber}", message.BookingNumber);

        try
        {
            var customerMessage = $"Your driver has arrived at the pickup location for booking {message.BookingNumber}. " +
                                $"Please proceed with loading. You have 2 hours of free time for loading.";

            await _notificationService.SendPushNotificationAsync(
                message.CustomerId,
                "Driver Arrived",
                customerMessage);

            await _notificationService.SendSmsAsync(
                message.CustomerId,
                customerMessage);

            _logger.LogInformation("DriverReached notification sent successfully for Booking {BookingNumber}", message.BookingNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending DriverReached notification for Booking {BookingNumber}", message.BookingNumber);
            throw;
        }
    }
}
