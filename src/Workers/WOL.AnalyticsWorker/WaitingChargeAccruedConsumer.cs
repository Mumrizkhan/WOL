using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using WOL.Shared.Messages.Events;

namespace WOL.AnalyticsWorker;

public class WaitingChargeAccruedConsumer : IConsumer<WaitingChargeAccruedEvent>
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly ILogger<WaitingChargeAccruedConsumer> _logger;

    public WaitingChargeAccruedConsumer(
        IMongoDatabase mongoDatabase,
        ILogger<WaitingChargeAccruedConsumer> logger)
    {
        _mongoDatabase = mongoDatabase;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<WaitingChargeAccruedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Recording waiting charge analytics for Booking {BookingId}", message.BookingId);

        try
        {
            var collection = _mongoDatabase.GetCollection<WaitingChargeAnalytics>("waiting_charge_analytics");

            var analytics = new WaitingChargeAnalytics
            {
                Id = Guid.NewGuid().ToString(),
                WaitingChargeId = message.WaitingChargeId,
                BookingId = message.BookingId,
                CustomerId = message.CustomerId,
                DriverId = message.DriverId,
                HoursCharged = message.HoursCharged,
                HourlyRate = message.HourlyRate,
                TotalAmount = message.TotalAmount,
                StartTime = message.StartTime,
                RecordedAt = DateTime.UtcNow
            };

            await collection.InsertOneAsync(analytics);
            _logger.LogInformation("Waiting charge analytics recorded for Booking {BookingId}", message.BookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording waiting charge analytics for Booking {BookingId}", message.BookingId);
            throw;
        }
    }
}

public class WaitingChargeAnalytics
{
    public string Id { get; set; } = string.Empty;
    public Guid WaitingChargeId { get; set; }
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid DriverId { get; set; }
    public int HoursCharged { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime RecordedAt { get; set; }
}
