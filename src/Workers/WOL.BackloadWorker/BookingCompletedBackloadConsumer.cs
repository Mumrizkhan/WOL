using MassTransit;
using WOL.Shared.Messages.Events;

namespace WOL.BackloadWorker;

public class BookingCompletedBackloadConsumer : IConsumer<BookingCompletedEvent>
{
    private readonly ILogger<BookingCompletedBackloadConsumer> _logger;

    public BookingCompletedBackloadConsumer(ILogger<BookingCompletedBackloadConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BookingCompletedEvent> context)
    {
        _logger.LogInformation("Processing backload opportunity for BookingCompletedEvent: {BookingId}", context.Message.BookingId);

        await Task.Delay(150);

        _logger.LogInformation("Backload opportunity created for Booking {BookingId}", context.Message.BookingId);
    }
}
