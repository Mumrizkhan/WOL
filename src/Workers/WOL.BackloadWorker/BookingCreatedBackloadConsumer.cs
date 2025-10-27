using MassTransit;
using WOL.Shared.Messages.Events;

namespace WOL.BackloadWorker;

public class BookingCreatedBackloadConsumer : IConsumer<BookingCreatedEvent>
{
    private readonly ILogger<BookingCreatedBackloadConsumer> _logger;

    public BookingCreatedBackloadConsumer(ILogger<BookingCreatedBackloadConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BookingCreatedEvent> context)
    {
        _logger.LogInformation("Processing backload matching for BookingCreatedEvent: {BookingId}", context.Message.BookingId);

        await Task.Delay(200);

        _logger.LogInformation("Backload matching completed for Booking {BookingId}", context.Message.BookingId);
    }
}
