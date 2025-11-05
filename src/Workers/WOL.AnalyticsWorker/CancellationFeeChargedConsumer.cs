using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using WOL.Shared.Messages.Events;

namespace WOL.AnalyticsWorker;

public class CancellationFeeChargedConsumer : IConsumer<CancellationFeeChargedEvent>
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly ILogger<CancellationFeeChargedConsumer> _logger;

    public CancellationFeeChargedConsumer(
        IMongoDatabase mongoDatabase,
        ILogger<CancellationFeeChargedConsumer> logger)
    {
        _mongoDatabase = mongoDatabase;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CancellationFeeChargedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Recording cancellation fee analytics for Booking {BookingId}", message.BookingId);

        try
        {
            var collection = _mongoDatabase.GetCollection<CancellationFeeAnalytics>("cancellation_fee_analytics");

            var analytics = new CancellationFeeAnalytics
            {
                Id = Guid.NewGuid().ToString(),
                CancellationFeeId = message.CancellationFeeId,
                BookingId = message.BookingId,
                Reason = message.Reason,
                CancelledBy = message.CancelledBy,
                ChargedTo = message.ChargedTo,
                ChargedToUserId = message.ChargedToUserId,
                Amount = message.Amount,
                CancelledAt = message.CancelledAt,
                RecordedAt = DateTime.UtcNow
            };

            await collection.InsertOneAsync(analytics);
            _logger.LogInformation("Cancellation fee analytics recorded for Booking {BookingId}", message.BookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording cancellation fee analytics for Booking {BookingId}", message.BookingId);
            throw;
        }
    }
}

public class CancellationFeeAnalytics
{
    public string Id { get; set; } = string.Empty;
    public Guid CancellationFeeId { get; set; }
    public Guid BookingId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string CancelledBy { get; set; } = string.Empty;
    public string ChargedTo { get; set; } = string.Empty;
    public Guid ChargedToUserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CancelledAt { get; set; }
    public DateTime RecordedAt { get; set; }
}
