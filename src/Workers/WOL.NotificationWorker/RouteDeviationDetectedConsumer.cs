using MassTransit;
using Microsoft.Extensions.Logging;
using WOL.NotificationWorker.Services;
using WOL.Shared.Messages.Events;

namespace WOL.NotificationWorker;

public class RouteDeviationDetectedConsumer : IConsumer<RouteDeviationDetectedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<RouteDeviationDetectedConsumer> _logger;

    public RouteDeviationDetectedConsumer(
        INotificationService notificationService,
        ILogger<RouteDeviationDetectedConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RouteDeviationDetectedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing route deviation alert for Booking {BookingId}", message.BookingId);

        try
        {
            var adminMessage = $"Route deviation detected for Booking {message.BookingId}. " +
                             $"Driver {message.DriverId} has deviated {message.DeviationDistanceKm:F1} km from expected route. " +
                             $"Reason: {message.Reason}";

            await _notificationService.SendPushNotificationAsync(
                message.DriverId,
                "Route Deviation Alert",
                "You have deviated from the expected route. Please return to the planned route.");

            _logger.LogInformation("Route deviation alert sent successfully for Booking {BookingId}", message.BookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending route deviation alert for Booking {BookingId}", message.BookingId);
            throw;
        }
    }
}
