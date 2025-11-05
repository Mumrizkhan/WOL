using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using WOL.Shared.Messages.Events;

namespace WOL.AnalyticsWorker;

public class LocationUpdatedConsumer : IConsumer<LocationUpdatedEvent>
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly ILogger<LocationUpdatedConsumer> _logger;

    public LocationUpdatedConsumer(
        IMongoDatabase mongoDatabase,
        ILogger<LocationUpdatedConsumer> logger)
    {
        _mongoDatabase = mongoDatabase;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LocationUpdatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Recording location analytics for Booking {BookingId}", message.BookingId);

        try
        {
            var collection = _mongoDatabase.GetCollection<LocationAnalytics>("location_analytics");

            var analytics = new LocationAnalytics
            {
                Id = Guid.NewGuid().ToString(),
                BookingId = message.BookingId,
                DriverId = message.DriverId,
                VehicleId = message.VehicleId,
                Latitude = message.Latitude,
                Longitude = message.Longitude,
                Speed = message.Speed,
                Heading = message.Heading,
                RecordedAt = message.Timestamp
            };

            await collection.InsertOneAsync(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording location analytics for Booking {BookingId}", message.BookingId);
            throw;
        }
    }
}

public class LocationAnalytics
{
    public string Id { get; set; } = string.Empty;
    public Guid BookingId { get; set; }
    public Guid DriverId { get; set; }
    public Guid VehicleId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Speed { get; set; }
    public double Heading { get; set; }
    public DateTime RecordedAt { get; set; }
}
