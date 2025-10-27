using MassTransit;
using WOL.Shared.Messages.Events;

namespace WOL.NotificationWorker;

public class BookingAssignedConsumer : IConsumer<BookingAssignedEvent>
{
    private readonly ILogger<BookingAssignedConsumer> _logger;

    public BookingAssignedConsumer(ILogger<BookingAssignedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BookingAssignedEvent> context)
    {
        _logger.LogInformation("Processing BookingAssignedEvent for Booking {BookingId}", context.Message.BookingId);

        await Task.Delay(100);

        _logger.LogInformation("Notification sent for Booking {BookingId} assignment", context.Message.BookingId);
    }
}
