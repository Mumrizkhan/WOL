using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using WOL.Shared.Messages.Events;

namespace WOL.AnalyticsWorker;

public class LoyaltyDiscountAppliedConsumer : IConsumer<LoyaltyDiscountAppliedEvent>
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly ILogger<LoyaltyDiscountAppliedConsumer> _logger;

    public LoyaltyDiscountAppliedConsumer(
        IMongoDatabase mongoDatabase,
        ILogger<LoyaltyDiscountAppliedConsumer> logger)
    {
        _mongoDatabase = mongoDatabase;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LoyaltyDiscountAppliedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Recording loyalty discount analytics for Customer {CustomerId}", message.CustomerId);

        try
        {
            var collection = _mongoDatabase.GetCollection<LoyaltyDiscountAnalytics>("loyalty_discount_analytics");

            var analytics = new LoyaltyDiscountAnalytics
            {
                Id = Guid.NewGuid().ToString(),
                BookingId = message.BookingId,
                CustomerId = message.CustomerId,
                TierLevel = message.TierLevel,
                TotalBookings = message.TotalBookings,
                TotalSpent = message.TotalSpent,
                DiscountPercentage = message.DiscountPercentage,
                DiscountAmount = message.DiscountAmount,
                OriginalAmount = message.OriginalAmount,
                FinalAmount = message.FinalAmount,
                RecordedAt = DateTime.UtcNow
            };

            await collection.InsertOneAsync(analytics);
            _logger.LogInformation("Loyalty discount analytics recorded for Customer {CustomerId}", message.CustomerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording loyalty discount analytics for Customer {CustomerId}", message.CustomerId);
            throw;
        }
    }
}

public class LoyaltyDiscountAnalytics
{
    public string Id { get; set; } = string.Empty;
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public string TierLevel { get; set; } = string.Empty;
    public int TotalBookings { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal OriginalAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public DateTime RecordedAt { get; set; }
}
