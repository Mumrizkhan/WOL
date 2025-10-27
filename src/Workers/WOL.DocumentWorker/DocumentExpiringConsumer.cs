using MassTransit;
using WOL.Shared.Messages.Events;
using WOL.DocumentWorker.Services;

namespace WOL.DocumentWorker;

public class DocumentExpiringConsumer : IConsumer<DocumentExpiringEvent>
{
    private readonly ILogger<DocumentExpiringConsumer> _logger;
    private readonly IDocumentProcessingService _documentProcessingService;

    public DocumentExpiringConsumer(
        ILogger<DocumentExpiringConsumer> logger,
        IDocumentProcessingService documentProcessingService)
    {
        _logger = logger;
        _documentProcessingService = documentProcessingService;
    }

    public async Task Consume(ConsumeContext<DocumentExpiringEvent> context)
    {
        _logger.LogInformation("Processing DocumentExpiringEvent for Document {DocumentId}", context.Message.DocumentId);

        var message = context.Message;

        await _documentProcessingService.SendExpiryNotificationAsync(
            message.DocumentId,
            message.DocumentType,
            message.ExpiryDate);

        _logger.LogInformation("Expiry notification sent for Document {DocumentId}", context.Message.DocumentId);
    }
}
