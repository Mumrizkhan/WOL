using MassTransit;
using WOL.Shared.Messages.Events;

namespace WOL.DocumentWorker;

public class DocumentUploadedConsumer : IConsumer<DocumentUploadedEvent>
{
    private readonly ILogger<DocumentUploadedConsumer> _logger;

    public DocumentUploadedConsumer(ILogger<DocumentUploadedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DocumentUploadedEvent> context)
    {
        _logger.LogInformation("Processing DocumentUploadedEvent for Document {DocumentId}", context.Message.DocumentId);

        await Task.Delay(200);

        _logger.LogInformation("OCR processing completed for Document {DocumentId}", context.Message.DocumentId);
    }
}
