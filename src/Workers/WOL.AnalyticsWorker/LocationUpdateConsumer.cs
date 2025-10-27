using MassTransit;
using WOL.Shared.Messages.Events;

namespace WOL.AnalyticsWorker;

public class LocationUpdateConsumer : IConsumer<LocationUpdateEvent>
{
    private readonly ILogger<LocationUpdateConsumer> _logger;

    public LocationUpdateConsumer(ILogger<LocationUpdateConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LocationUpdateEvent> context)
    {
        _logger.LogInformation("Processing analytics for LocationUpdateEvent: {VehicleId}", context.Message.VehicleId);

        await Task.Delay(50);

        _logger.LogInformation("Analytics recorded for Vehicle {VehicleId}", context.Message.VehicleId);
    }
}
