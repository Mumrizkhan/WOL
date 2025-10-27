using MassTransit;
using WOL.Shared.Messages.Events;

namespace WOL.ReportingWorker;

public class PaymentProcessedReportConsumer : IConsumer<PaymentProcessedEvent>
{
    private readonly ILogger<PaymentProcessedReportConsumer> _logger;

    public PaymentProcessedReportConsumer(ILogger<PaymentProcessedReportConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        _logger.LogInformation("Processing report data for PaymentProcessedEvent: {PaymentId}", context.Message.PaymentId);

        await Task.Delay(100);

        _logger.LogInformation("Report data recorded for Payment {PaymentId}", context.Message.PaymentId);
    }
}
