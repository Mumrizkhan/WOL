using MassTransit;
using WOL.Shared.Messages.Events;
using WOL.ReportingWorker.Services;

namespace WOL.ReportingWorker;

public class PaymentProcessedReportConsumer : IConsumer<PaymentProcessedEvent>
{
    private readonly ILogger<PaymentProcessedReportConsumer> _logger;
    private readonly IReportingService _reportingService;

    public PaymentProcessedReportConsumer(
        ILogger<PaymentProcessedReportConsumer> logger,
        IReportingService reportingService)
    {
        _logger = logger;
        _reportingService = reportingService;
    }

    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        _logger.LogInformation("Processing report data for PaymentProcessedEvent: {PaymentId}", context.Message.PaymentId);

        var message = context.Message;

        await _reportingService.AggregatePaymentDataAsync(
            message.PaymentId,
            message.BookingId,
            message.Amount,
            message.PaymentMethod,
            message.ProcessedAt);

        _logger.LogInformation("Report data aggregated for Payment {PaymentId}", context.Message.PaymentId);
    }
}
