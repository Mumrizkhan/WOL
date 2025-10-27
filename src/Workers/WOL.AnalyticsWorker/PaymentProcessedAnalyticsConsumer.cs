using MassTransit;
using WOL.Shared.Messages.Events;

namespace WOL.AnalyticsWorker;

public class PaymentProcessedAnalyticsConsumer : IConsumer<PaymentProcessedEvent>
{
    private readonly ILogger<PaymentProcessedAnalyticsConsumer> _logger;

    public PaymentProcessedAnalyticsConsumer(ILogger<PaymentProcessedAnalyticsConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        _logger.LogInformation("Processing analytics for PaymentProcessedEvent: {PaymentId}", context.Message.PaymentId);

        await Task.Delay(100);

        _logger.LogInformation("Analytics recorded for Payment {PaymentId}", context.Message.PaymentId);
    }
}
