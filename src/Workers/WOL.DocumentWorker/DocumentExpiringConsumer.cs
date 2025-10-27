using MassTransit;
using WOL.Shared.Messages.Events;

namespace WOL.DocumentWorker;

public class DocumentExpiringConsumer : IConsumer<DocumentExpiringEvent>
{
    private readonly ILogger<DocumentExpiringConsumer> _logger;

    public DocumentExpiringConsumer(ILogger<DocumentExpiringConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DocumentExpiringEvent> context)
    {
        _logger.LogInformation("Processing DocumentExpiringEvent for Document {DocumentId}", context.Message.DocumentId);

        await Task.Delay(100);

        _logger.LogInformation("Expiry notification sent for Document {DocumentId}", context.Message.DocumentId);
    }
}
