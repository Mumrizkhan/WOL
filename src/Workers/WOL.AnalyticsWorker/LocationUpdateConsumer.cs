using MassTransit;
using WOL.Shared.Messages.Events;
using WOL.AnalyticsWorker.Services;

namespace WOL.AnalyticsWorker;

public class LocationUpdateConsumer : IConsumer<LocationUpdatedEvent>
{
    private readonly ILogger<LocationUpdateConsumer> _logger;
    private readonly IAnalyticsService _analyticsService;

    public LocationUpdateConsumer(
        ILogger<LocationUpdateConsumer> logger,
        IAnalyticsService analyticsService)
    {
        _logger = logger;
        _analyticsService = analyticsService;
    }

    public async Task Consume(ConsumeContext<LocationUpdatedEvent> context)
    {
        _logger.LogDebug("Processing location update for Booking: {BookingId}", context.Message.BookingId);

        var message = context.Message;

        await _analyticsService.RecordLocationUpdateAsync(
            message.BookingId,
            message.DriverId,
            message.Latitude,
            message.Longitude,
            message.Timestamp);
    }
}
