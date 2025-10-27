using MongoDB.Driver;
using MongoDB.Bson;

namespace WOL.AnalyticsWorker.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly ILogger<AnalyticsService> _logger;
    private readonly IMongoDatabase _database;

    public AnalyticsService(ILogger<AnalyticsService> logger, IMongoDatabase database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task RecordBookingCreatedAsync(Guid bookingId, Guid customerId, string bookingType, decimal estimatedPrice, DateTime createdAt)
    {
        try
        {
            var collection = _database.GetCollection<BsonDocument>("booking_analytics");
            
            var document = new BsonDocument
            {
                { "bookingId", bookingId.ToString() },
                { "customerId", customerId.ToString() },
                { "bookingType", bookingType },
                { "estimatedPrice", estimatedPrice },
                { "createdAt", createdAt },
                { "eventType", "booking_created" },
                { "recordedAt", DateTime.UtcNow }
            };

            await collection.InsertOneAsync(document);
            _logger.LogInformation("Recorded booking created analytics for {BookingId}", bookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording booking created analytics for {BookingId}", bookingId);
        }
    }

    public async Task RecordPaymentProcessedAsync(Guid paymentId, Guid bookingId, decimal amount, string paymentMethod, DateTime processedAt)
    {
        try
        {
            var collection = _database.GetCollection<BsonDocument>("payment_analytics");
            
            var document = new BsonDocument
            {
                { "paymentId", paymentId.ToString() },
                { "bookingId", bookingId.ToString() },
                { "amount", amount },
                { "paymentMethod", paymentMethod },
                { "processedAt", processedAt },
                { "eventType", "payment_processed" },
                { "recordedAt", DateTime.UtcNow }
            };

            await collection.InsertOneAsync(document);
            _logger.LogInformation("Recorded payment analytics for {PaymentId}", paymentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording payment analytics for {PaymentId}", paymentId);
        }
    }

    public async Task RecordLocationUpdateAsync(Guid bookingId, Guid driverId, double latitude, double longitude, DateTime timestamp)
    {
        try
        {
            var collection = _database.GetCollection<BsonDocument>("location_history");
            
            var document = new BsonDocument
            {
                { "bookingId", bookingId.ToString() },
                { "driverId", driverId.ToString() },
                { "location", new BsonDocument
                    {
                        { "type", "Point" },
                        { "coordinates", new BsonArray { longitude, latitude } }
                    }
                },
                { "timestamp", timestamp },
                { "recordedAt", DateTime.UtcNow }
            };

            await collection.InsertOneAsync(document);
            _logger.LogDebug("Recorded location update for booking {BookingId}", bookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording location update for {BookingId}", bookingId);
        }
    }
}
