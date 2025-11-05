using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using WOL.Shared.Messages.Events;

namespace WOL.AnalyticsWorker;

public class PaymentDeadlineExpiredConsumer : IConsumer<PaymentDeadlineExpiredEvent>
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly ILogger<PaymentDeadlineExpiredConsumer> _logger;

    public PaymentDeadlineExpiredConsumer(
        IMongoDatabase mongoDatabase,
        ILogger<PaymentDeadlineExpiredConsumer> logger)
    {
        _mongoDatabase = mongoDatabase;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentDeadlineExpiredEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Recording payment deadline expiry analytics for Booking {BookingId}", message.BookingId);

        try
        {
            var collection = _mongoDatabase.GetCollection<PaymentDeadlineAnalytics>("payment_deadline_analytics");

            var analytics = new PaymentDeadlineAnalytics
            {
                Id = Guid.NewGuid().ToString(),
                PaymentDeadlineId = message.PaymentDeadlineId,
                BookingId = message.BookingId,
                CustomerId = message.CustomerId,
                CustomerType = message.CustomerType,
                Amount = message.Amount,
                DeadlineAt = message.DeadlineAt,
                ExpiredAt = message.ExpiredAt,
                RecordedAt = DateTime.UtcNow
            };

            await collection.InsertOneAsync(analytics);
            _logger.LogInformation("Payment deadline expiry analytics recorded for Booking {BookingId}", message.BookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording payment deadline expiry analytics for Booking {BookingId}", message.BookingId);
            throw;
        }
    }
}

public class PaymentDeadlineAnalytics
{
    public string Id { get; set; } = string.Empty;
    public Guid PaymentDeadlineId { get; set; }
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime DeadlineAt { get; set; }
    public DateTime ExpiredAt { get; set; }
    public DateTime RecordedAt { get; set; }
}
