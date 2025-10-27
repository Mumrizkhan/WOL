using MassTransit;
using WOL.Shared.Messages.Events;
using WOL.AnalyticsWorker.Services;

namespace WOL.AnalyticsWorker;

public class PaymentProcessedAnalyticsConsumer : IConsumer<PaymentProcessedEvent>
{
    private readonly ILogger<PaymentProcessedAnalyticsConsumer> _logger;
    private readonly IAnalyticsService _analyticsService;

    public PaymentProcessedAnalyticsConsumer(
        ILogger<PaymentProcessedAnalyticsConsumer> logger,
        IAnalyticsService analyticsService)
    {
        _logger = logger;
        _analyticsService = analyticsService;
    }

    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        _logger.LogInformation("Processing analytics for PaymentProcessedEvent: {PaymentId}", context.Message.PaymentId);

        var message = context.Message;

        await _analyticsService.RecordPaymentProcessedAsync(
            message.PaymentId,
            message.BookingId,
            message.Amount,
            message.PaymentMethod,
            message.ProcessedAt);

        _logger.LogInformation("Analytics recorded for Payment {PaymentId}", context.Message.PaymentId);
    }
}
