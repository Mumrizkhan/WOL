using MassTransit;
using Microsoft.Extensions.Logging;
using WOL.NotificationWorker.Services;
using WOL.Shared.Messages.Events;

namespace WOL.NotificationWorker;

public class SharedLoadCapacityUpdatedConsumer : IConsumer<SharedLoadCapacityUpdatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<SharedLoadCapacityUpdatedConsumer> _logger;

    public SharedLoadCapacityUpdatedConsumer(
        INotificationService notificationService,
        ILogger<SharedLoadCapacityUpdatedConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SharedLoadCapacityUpdatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing shared load capacity update for SharedLoad {SharedLoadBookingId}", 
            message.SharedLoadBookingId);

        try
        {
            if (message.Status == "Full")
            {
                var driverMessage = $"Your shared load from {message.OriginCity} to {message.DestinationCity} is now full. " +
                                  $"Capacity utilization: {message.CapacityUtilization:F1}%. " +
                                  $"Total bookings: {message.BookingCount}.";

                await _notificationService.SendPushNotificationAsync(
                    message.DriverId,
                    "Shared Load Full",
                    driverMessage);
            }

            _logger.LogInformation("Shared load capacity notification sent successfully for SharedLoad {SharedLoadBookingId}", 
                message.SharedLoadBookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending shared load capacity notification for SharedLoad {SharedLoadBookingId}", 
                message.SharedLoadBookingId);
            throw;
        }
    }
}
