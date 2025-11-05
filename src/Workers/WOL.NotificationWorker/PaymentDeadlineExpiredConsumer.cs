using MassTransit;
using Microsoft.Extensions.Logging;
using WOL.NotificationWorker.Services;
using WOL.Shared.Messages.Events;

namespace WOL.NotificationWorker;

public class PaymentDeadlineExpiredConsumer : IConsumer<PaymentDeadlineExpiredEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<PaymentDeadlineExpiredConsumer> _logger;

    public PaymentDeadlineExpiredConsumer(
        INotificationService notificationService,
        ILogger<PaymentDeadlineExpiredConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentDeadlineExpiredEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing PaymentDeadlineExpired notification for Booking {BookingId}", message.BookingId);

        try
        {
            var notificationMessage = $"Your payment deadline for booking has expired. " +
                                    $"Amount: SAR {message.Amount:F2}. " +
                                    $"The booking has been automatically cancelled and refund initiated.";

            await _notificationService.SendPushNotificationAsync(
                message.CustomerId,
                "Payment Deadline Expired",
                notificationMessage);

            await _notificationService.SendSmsAsync(
                message.CustomerId,
                notificationMessage);

            _logger.LogInformation("PaymentDeadlineExpired notification sent successfully for Booking {BookingId}", message.BookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending PaymentDeadlineExpired notification for Booking {BookingId}", message.BookingId);
            throw;
        }
    }
}
