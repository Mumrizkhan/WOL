using MassTransit;
using WOL.Shared.Messages.Events;

namespace WOL.NotificationWorker;

public class BookingCreatedConsumer : IConsumer<BookingCreatedEvent>
{
    private readonly ILogger<BookingCreatedConsumer> _logger;

    public BookingCreatedConsumer(ILogger<BookingCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BookingCreatedEvent> context)
    {
        _logger.LogInformation("Processing BookingCreatedEvent for Booking {BookingId}", context.Message.BookingId);

        await Task.Delay(100);

        _logger.LogInformation("Notification sent for Booking {BookingId}", context.Message.BookingId);
    }
}
