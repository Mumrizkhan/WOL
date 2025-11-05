using MassTransit;
using Microsoft.Extensions.Logging;
using WOL.NotificationWorker.Services;
using WOL.Shared.Messages.Events;

namespace WOL.NotificationWorker;

public class DriverPayoutProcessedConsumer : IConsumer<DriverPayoutProcessedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<DriverPayoutProcessedConsumer> _logger;

    public DriverPayoutProcessedConsumer(
        INotificationService notificationService,
        ILogger<DriverPayoutProcessedConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DriverPayoutProcessedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing DriverPayoutProcessed notification for Payout {PayoutId}", message.PayoutId);

        try
        {
            var notificationMessage = $"Your payout for period {message.PeriodStart:yyyy-MM-dd} to {message.PeriodEnd:yyyy-MM-dd} has been processed. " +
                                    $"Total Earnings: SAR {message.TotalEarnings:F2}. " +
                                    $"Penalties: SAR {message.TotalPenalties:F2}. " +
                                    $"Net Payout: SAR {message.NetPayout:F2}.";

            await _notificationService.SendPushNotificationAsync(
                message.DriverId,
                "Payout Processed",
                notificationMessage);

            await _notificationService.SendSmsAsync(
                message.DriverId,
                notificationMessage);

            _logger.LogInformation("DriverPayoutProcessed notification sent successfully for Payout {PayoutId}", message.PayoutId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending DriverPayoutProcessed notification for Payout {PayoutId}", message.PayoutId);
            throw;
        }
    }
}
