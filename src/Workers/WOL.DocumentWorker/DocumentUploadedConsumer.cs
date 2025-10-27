using MassTransit;
using WOL.Shared.Messages.Events;
using WOL.DocumentWorker.Services;

namespace WOL.DocumentWorker;

public class DocumentUploadedConsumer : IConsumer<DocumentUploadedEvent>
{
    private readonly ILogger<DocumentUploadedConsumer> _logger;
    private readonly IDocumentProcessingService _documentProcessingService;

    public DocumentUploadedConsumer(
        ILogger<DocumentUploadedConsumer> logger,
        IDocumentProcessingService documentProcessingService)
    {
        _logger = logger;
        _documentProcessingService = documentProcessingService;
    }

    public async Task Consume(ConsumeContext<DocumentUploadedEvent> context)
    {
        _logger.LogInformation("Processing DocumentUploadedEvent for Document {DocumentId}", context.Message.DocumentId);

        var message = context.Message;

        await _documentProcessingService.ProcessDocumentAsync(
            message.DocumentId,
            message.DocumentType,
            message.FilePath);

        _logger.LogInformation("OCR processing completed for Document {DocumentId}", context.Message.DocumentId);
    }
}
