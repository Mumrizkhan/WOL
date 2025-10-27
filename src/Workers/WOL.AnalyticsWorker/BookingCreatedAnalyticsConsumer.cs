using MassTransit;
using WOL.Shared.Messages.Events;
using WOL.AnalyticsWorker.Services;

namespace WOL.AnalyticsWorker;

public class BookingCreatedAnalyticsConsumer : IConsumer<BookingCreatedEvent>
{
    private readonly ILogger<BookingCreatedAnalyticsConsumer> _logger;
    private readonly IAnalyticsService _analyticsService;

    public BookingCreatedAnalyticsConsumer(
        ILogger<BookingCreatedAnalyticsConsumer> logger,
        IAnalyticsService analyticsService)
    {
        _logger = logger;
        _analyticsService = analyticsService;
    }

    public async Task Consume(ConsumeContext<BookingCreatedEvent> context)
    {
        _logger.LogInformation("Processing analytics for BookingCreatedEvent: {BookingId}", context.Message.BookingId);

        var message = context.Message;

        await _analyticsService.RecordBookingCreatedAsync(
            message.BookingId,
            message.CustomerId,
            message.BookingType,
            message.EstimatedPrice,
            message.CreatedAt);

        _logger.LogInformation("Analytics recorded for Booking {BookingId}", context.Message.BookingId);
    }
}
