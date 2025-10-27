using MassTransit;
using WOL.Shared.Messages.Events;
using WOL.NotificationWorker.Services;

namespace WOL.NotificationWorker;

public class BookingCompletedConsumer : IConsumer<BookingCompletedEvent>
{
    private readonly ILogger<BookingCompletedConsumer> _logger;
    private readonly INotificationService _notificationService;

    public BookingCompletedConsumer(
        ILogger<BookingCompletedConsumer> logger,
        INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task Consume(ConsumeContext<BookingCompletedEvent> context)
    {
        _logger.LogInformation("Processing BookingCompletedEvent for Booking {BookingId}", context.Message.BookingId);

        var message = context.Message;

        await _notificationService.SendPushNotificationAsync(
            message.CustomerId.ToString(),
            "Delivery Completed",
            $"Your booking #{message.BookingId} has been delivered successfully!",
            new Dictionary<string, string>
            {
                { "bookingId", message.BookingId.ToString() },
                { "type", "booking_completed" }
            });

        if (!string.IsNullOrEmpty(message.CustomerEmail))
        {
            var emailBody = $@"
                <h2>Delivery Completed</h2>
                <p>Dear Customer,</p>
                <p>Your booking has been delivered successfully!</p>
                <p><strong>Booking ID:</strong> {message.BookingId}</p>
                <p><strong>Completed At:</strong> {message.CompletedAt:yyyy-MM-dd HH:mm}</p>
                <p>Thank you for using World of Logistics. We hope to serve you again!</p>
                <p>Please rate your experience in the app.</p>";

            await _notificationService.SendEmailAsync(
                message.CustomerEmail,
                "Delivery Completed - World of Logistics",
                emailBody);
        }

        await _notificationService.SendPushNotificationAsync(
            message.DriverId.ToString(),
            "Job Completed",
            $"Booking #{message.BookingId} has been marked as completed.",
            new Dictionary<string, string>
            {
                { "bookingId", message.BookingId.ToString() },
                { "type", "job_completed" }
            });

        _logger.LogInformation("Notifications sent for Booking {BookingId} completion", context.Message.BookingId);
    }
}
