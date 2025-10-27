using MassTransit;
using WOL.Shared.Messages.Events;
using WOL.NotificationWorker.Services;

namespace WOL.NotificationWorker;

public class BookingCreatedConsumer : IConsumer<BookingCreatedEvent>
{
    private readonly ILogger<BookingCreatedConsumer> _logger;
    private readonly INotificationService _notificationService;

    public BookingCreatedConsumer(
        ILogger<BookingCreatedConsumer> logger,
        INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task Consume(ConsumeContext<BookingCreatedEvent> context)
    {
        _logger.LogInformation("Processing BookingCreatedEvent for Booking {BookingId}", context.Message.BookingId);

        var message = context.Message;

        await _notificationService.SendPushNotificationAsync(
            message.CustomerId.ToString(),
            "Booking Created",
            $"Your booking #{message.BookingId} has been created successfully.",
            new Dictionary<string, string>
            {
                { "bookingId", message.BookingId.ToString() },
                { "type", "booking_created" }
            });

        if (!string.IsNullOrEmpty(message.CustomerPhone))
        {
            await _notificationService.SendSmsAsync(
                message.CustomerPhone,
                $"Your booking #{message.BookingId} has been created. Track your shipment in the WOL app.");
        }

        if (!string.IsNullOrEmpty(message.CustomerEmail))
        {
            var emailBody = $@"
                <h2>Booking Confirmation</h2>
                <p>Dear Customer,</p>
                <p>Your booking has been created successfully.</p>
                <p><strong>Booking ID:</strong> {message.BookingId}</p>
                <p><strong>Pickup Location:</strong> {message.PickupLocation}</p>
                <p><strong>Delivery Location:</strong> {message.DeliveryLocation}</p>
                <p>You can track your shipment in the World of Logistics mobile app.</p>
                <p>Thank you for choosing World of Logistics!</p>";

            await _notificationService.SendEmailAsync(
                message.CustomerEmail,
                "Booking Confirmation - World of Logistics",
                emailBody);
        }

        _logger.LogInformation("Notifications sent for Booking {BookingId}", context.Message.BookingId);
    }
}
