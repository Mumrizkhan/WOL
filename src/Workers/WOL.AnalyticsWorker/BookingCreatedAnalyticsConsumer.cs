using MassTransit;
using WOL.Shared.Messages.Events;

namespace WOL.AnalyticsWorker;

public class BookingCreatedAnalyticsConsumer : IConsumer<BookingCreatedEvent>
{
    private readonly ILogger<BookingCreatedAnalyticsConsumer> _logger;

    public BookingCreatedAnalyticsConsumer(ILogger<BookingCreatedAnalyticsConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BookingCreatedEvent> context)
    {
        _logger.LogInformation("Processing analytics for BookingCreatedEvent: {BookingId}", context.Message.BookingId);

        await Task.Delay(100);

        _logger.LogInformation("Analytics recorded for Booking {BookingId}", context.Message.BookingId);
    }
}
