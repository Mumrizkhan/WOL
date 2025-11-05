using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using WOL.Shared.Messages.Events;

namespace WOL.AnalyticsWorker;

public class SurgePricingAppliedConsumer : IConsumer<SurgePricingAppliedEvent>
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly ILogger<SurgePricingAppliedConsumer> _logger;

    public SurgePricingAppliedConsumer(
        IMongoDatabase mongoDatabase,
        ILogger<SurgePricingAppliedConsumer> logger)
    {
        _mongoDatabase = mongoDatabase;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SurgePricingAppliedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Recording surge pricing analytics for Booking {BookingId}", message.BookingId);

        try
        {
            var collection = _mongoDatabase.GetCollection<SurgePricingAnalytics>("surge_pricing_analytics");

            var analytics = new SurgePricingAnalytics
            {
                Id = Guid.NewGuid().ToString(),
                BookingId = message.BookingId,
                CustomerId = message.CustomerId,
                City = message.City,
                BaseAmount = message.BaseAmount,
                Multiplier = message.Multiplier,
                SurgeAmount = message.SurgeAmount,
                FinalAmount = message.FinalAmount,
                RecordedAt = DateTime.UtcNow
            };

            await collection.InsertOneAsync(analytics);
            _logger.LogInformation("Surge pricing analytics recorded for Booking {BookingId}", message.BookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording surge pricing analytics for Booking {BookingId}", message.BookingId);
            throw;
        }
    }
}

public class SurgePricingAnalytics
{
    public string Id { get; set; } = string.Empty;
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public string City { get; set; } = string.Empty;
    public decimal BaseAmount { get; set; }
    public decimal Multiplier { get; set; }
    public decimal SurgeAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public DateTime RecordedAt { get; set; }
}
