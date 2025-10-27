using MassTransit;
using WOL.Shared.Messages.Events;
using WOL.NotificationWorker.Services;

namespace WOL.NotificationWorker;

public class BookingAssignedConsumer : IConsumer<BookingAssignedEvent>
{
    private readonly ILogger<BookingAssignedConsumer> _logger;
    private readonly INotificationService _notificationService;

    public BookingAssignedConsumer(
        ILogger<BookingAssignedConsumer> logger,
        INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task Consume(ConsumeContext<BookingAssignedEvent> context)
    {
        _logger.LogInformation("Processing BookingAssignedEvent for Booking {BookingId}", context.Message.BookingId);

        var message = context.Message;

        await _notificationService.SendPushNotificationAsync(
            message.CustomerId.ToString(),
            "Driver Assigned",
            $"A driver has been assigned to your booking #{message.BookingId}.",
            new Dictionary<string, string>
            {
                { "bookingId", message.BookingId.ToString() },
                { "driverId", message.DriverId.ToString() },
                { "type", "booking_assigned" }
            });

        await _notificationService.SendPushNotificationAsync(
            message.DriverId.ToString(),
            "New Job Assigned",
            $"You have been assigned to booking #{message.BookingId}.",
            new Dictionary<string, string>
            {
                { "bookingId", message.BookingId.ToString() },
                { "type", "job_assigned" }
            });

        if (!string.IsNullOrEmpty(message.DriverPhone))
        {
            await _notificationService.SendSmsAsync(
                message.DriverPhone,
                $"New job assigned! Booking #{message.BookingId}. Check the WOL Driver app for details.");
        }

        _logger.LogInformation("Notifications sent for Booking {BookingId} assignment", context.Message.BookingId);
    }
}
