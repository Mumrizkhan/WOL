using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using WOL.Shared.Messages.Events;

namespace WOL.AnalyticsWorker;

public class DriverPayoutProcessedConsumer : IConsumer<DriverPayoutProcessedEvent>
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly ILogger<DriverPayoutProcessedConsumer> _logger;

    public DriverPayoutProcessedConsumer(
        IMongoDatabase mongoDatabase,
        ILogger<DriverPayoutProcessedConsumer> logger)
    {
        _mongoDatabase = mongoDatabase;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DriverPayoutProcessedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Recording driver payout analytics for Payout {PayoutId}", message.PayoutId);

        try
        {
            var collection = _mongoDatabase.GetCollection<DriverPayoutAnalytics>("driver_payout_analytics");

            var analytics = new DriverPayoutAnalytics
            {
                Id = Guid.NewGuid().ToString(),
                PayoutId = message.PayoutId,
                DriverId = message.DriverId,
                PeriodStart = message.PeriodStart,
                PeriodEnd = message.PeriodEnd,
                TotalEarnings = message.TotalEarnings,
                TotalPenalties = message.TotalPenalties,
                NetPayout = message.NetPayout,
                ProcessedAt = message.ProcessedAt,
                ProcessedBy = message.ProcessedBy,
                RecordedAt = DateTime.UtcNow
            };

            await collection.InsertOneAsync(analytics);
            _logger.LogInformation("Driver payout analytics recorded for Payout {PayoutId}", message.PayoutId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording driver payout analytics for Payout {PayoutId}", message.PayoutId);
            throw;
        }
    }
}

public class DriverPayoutAnalytics
{
    public string Id { get; set; } = string.Empty;
    public Guid PayoutId { get; set; }
    public Guid DriverId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalPenalties { get; set; }
    public decimal NetPayout { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string ProcessedBy { get; set; } = string.Empty;
    public DateTime RecordedAt { get; set; }
}
