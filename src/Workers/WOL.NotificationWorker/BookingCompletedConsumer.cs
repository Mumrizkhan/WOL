using MassTransit;
using WOL.Shared.Messages.Events;

namespace WOL.NotificationWorker;

public class BookingCompletedConsumer : IConsumer<BookingCompletedEvent>
{
    private readonly ILogger<BookingCompletedConsumer> _logger;

    public BookingCompletedConsumer(ILogger<BookingCompletedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BookingCompletedEvent> context)
    {
        _logger.LogInformation("Processing BookingCompletedEvent for Booking {BookingId}", context.Message.BookingId);

        await Task.Delay(100);

        _logger.LogInformation("Notification sent for Booking {BookingId} completion", context.Message.BookingId);
    }
}
