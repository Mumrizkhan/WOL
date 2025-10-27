using MassTransit;
using WOL.Shared.Messages.Events;
using WOL.ComplianceWorker.Services;

namespace WOL.ComplianceWorker;

public class DocumentExpiringComplianceConsumer : IConsumer<DocumentExpiringEvent>
{
    private readonly ILogger<DocumentExpiringComplianceConsumer> _logger;
    private readonly IComplianceService _complianceService;

    public DocumentExpiringComplianceConsumer(
        ILogger<DocumentExpiringComplianceConsumer> logger,
        IComplianceService complianceService)
    {
        _logger = logger;
        _complianceService = complianceService;
    }

    public async Task Consume(ConsumeContext<DocumentExpiringEvent> context)
    {
        _logger.LogInformation("Processing compliance check for Document {DocumentId}", context.Message.DocumentId);

        var message = context.Message;

        await _complianceService.CheckDocumentComplianceAsync(
            message.DocumentId,
            message.DocumentType,
            message.ExpiryDate);

        _logger.LogInformation("Document compliance check completed for {DocumentId}", context.Message.DocumentId);
    }
}
