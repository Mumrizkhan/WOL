using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using WOL.Shared.Messages.Events;

namespace WOL.AnalyticsWorker;

public class BackloadDiscountAppliedConsumer : IConsumer<BackloadDiscountAppliedEvent>
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly ILogger<BackloadDiscountAppliedConsumer> _logger;

    public BackloadDiscountAppliedConsumer(
        IMongoDatabase mongoDatabase,
        ILogger<BackloadDiscountAppliedConsumer> logger)
    {
        _mongoDatabase = mongoDatabase;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BackloadDiscountAppliedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Recording backload discount analytics for Booking {BookingId}", message.BookingId);

        try
        {
            var collection = _mongoDatabase.GetCollection<BackloadDiscountAnalytics>("backload_discount_analytics");

            var analytics = new BackloadDiscountAnalytics
            {
                Id = Guid.NewGuid().ToString(),
                BookingId = message.BookingId,
                CustomerId = message.CustomerId,
                BackloadOpportunityId = message.BackloadOpportunityId,
                OriginalFare = message.OriginalFare,
                DiscountPercentage = message.DiscountPercentage,
                DiscountAmount = message.DiscountAmount,
                FinalFare = message.FinalFare,
                RecordedAt = DateTime.UtcNow
            };

            await collection.InsertOneAsync(analytics);
            _logger.LogInformation("Backload discount analytics recorded for Booking {BookingId}", message.BookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording backload discount analytics for Booking {BookingId}", message.BookingId);
            throw;
        }
    }
}

public class BackloadDiscountAnalytics
{
    public string Id { get; set; } = string.Empty;
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? BackloadOpportunityId { get; set; }
    public decimal OriginalFare { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalFare { get; set; }
    public DateTime RecordedAt { get; set; }
}
