using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using WOL.Shared.Messages.Events;

namespace WOL.AnalyticsWorker;

public class RouteUtilizationUpdatedConsumer : IConsumer<RouteUtilizationUpdatedEvent>
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly ILogger<RouteUtilizationUpdatedConsumer> _logger;

    public RouteUtilizationUpdatedConsumer(
        IMongoDatabase mongoDatabase,
        ILogger<RouteUtilizationUpdatedConsumer> logger)
    {
        _mongoDatabase = mongoDatabase;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RouteUtilizationUpdatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Recording route utilization analytics for {Origin} to {Destination}", 
            message.OriginCity, message.DestinationCity);

        try
        {
            var collection = _mongoDatabase.GetCollection<RouteUtilizationAnalytics>("route_utilization_analytics");

            var analytics = new RouteUtilizationAnalytics
            {
                Id = Guid.NewGuid().ToString(),
                RouteUtilizationId = message.RouteUtilizationId,
                OriginCity = message.OriginCity,
                DestinationCity = message.DestinationCity,
                OutboundBookings = message.OutboundBookings,
                ReturnBookings = message.ReturnBookings,
                UtilizationPercentage = message.UtilizationPercentage,
                EmptyKmTotal = message.EmptyKmTotal,
                EmptyKmSaved = message.EmptyKmSaved,
                PeriodStart = message.PeriodStart,
                PeriodEnd = message.PeriodEnd,
                RecordedAt = DateTime.UtcNow
            };

            await collection.InsertOneAsync(analytics);
            _logger.LogInformation("Route utilization analytics recorded for {Origin} to {Destination}", 
                message.OriginCity, message.DestinationCity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording route utilization analytics for {Origin} to {Destination}", 
                message.OriginCity, message.DestinationCity);
            throw;
        }
    }
}

public class RouteUtilizationAnalytics
{
    public string Id { get; set; } = string.Empty;
    public Guid RouteUtilizationId { get; set; }
    public string OriginCity { get; set; } = string.Empty;
    public string DestinationCity { get; set; } = string.Empty;
    public int OutboundBookings { get; set; }
    public int ReturnBookings { get; set; }
    public decimal UtilizationPercentage { get; set; }
    public decimal EmptyKmTotal { get; set; }
    public decimal EmptyKmSaved { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime RecordedAt { get; set; }
}
