using MassTransit;
using WOL.Shared.Messages.Events;

namespace WOL.ComplianceWorker;

public class DocumentExpiringComplianceConsumer : IConsumer<DocumentExpiringEvent>
{
    private readonly ILogger<DocumentExpiringComplianceConsumer> _logger;

    public DocumentExpiringComplianceConsumer(ILogger<DocumentExpiringComplianceConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DocumentExpiringEvent> context)
    {
        _logger.LogInformation("Processing compliance check for DocumentExpiringEvent: {DocumentId}", context.Message.DocumentId);

        await Task.Delay(100);

        _logger.LogInformation("Compliance check completed for Document {DocumentId}", context.Message.DocumentId);
    }
}
