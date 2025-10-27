using MongoDB.Driver;
using MongoDB.Bson;

namespace WOL.ReportingWorker.Services;

public class ReportingService : IReportingService
{
    private readonly ILogger<ReportingService> _logger;
    private readonly IMongoDatabase _database;

    public ReportingService(ILogger<ReportingService> logger, IMongoDatabase database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task AggregateBookingDataAsync(Guid bookingId, Guid customerId, decimal totalAmount, DateTime completedAt)
    {
        try
        {
            var collection = _database.GetCollection<BsonDocument>("booking_reports");
            
            var document = new BsonDocument
            {
                { "bookingId", bookingId.ToString() },
                { "customerId", customerId.ToString() },
                { "totalAmount", totalAmount },
                { "completedAt", completedAt },
                { "year", completedAt.Year },
                { "month", completedAt.Month },
                { "day", completedAt.Day },
                { "aggregatedAt", DateTime.UtcNow }
            };

            await collection.InsertOneAsync(document);

            await UpdateDailyRevenueAsync(completedAt, totalAmount);
            await UpdateMonthlyRevenueAsync(completedAt, totalAmount);

            _logger.LogInformation("Aggregated booking data for {BookingId}", bookingId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error aggregating booking data for {BookingId}", bookingId);
        }
    }

    public async Task AggregatePaymentDataAsync(Guid paymentId, Guid bookingId, decimal amount, string paymentMethod, DateTime processedAt)
    {
        try
        {
            var collection = _database.GetCollection<BsonDocument>("payment_reports");
            
            var document = new BsonDocument
            {
                { "paymentId", paymentId.ToString() },
                { "bookingId", bookingId.ToString() },
                { "amount", amount },
                { "paymentMethod", paymentMethod },
                { "processedAt", processedAt },
                { "year", processedAt.Year },
                { "month", processedAt.Month },
                { "day", processedAt.Day },
                { "aggregatedAt", DateTime.UtcNow }
            };

            await collection.InsertOneAsync(document);

            await UpdatePaymentMethodStatsAsync(paymentMethod, amount, processedAt);

            _logger.LogInformation("Aggregated payment data for {PaymentId}", paymentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error aggregating payment data for {PaymentId}", paymentId);
        }
    }

    private async Task UpdateDailyRevenueAsync(DateTime date, decimal amount)
    {
        try
        {
            var collection = _database.GetCollection<BsonDocument>("daily_revenue");
            var dateKey = date.ToString("yyyy-MM-dd");

            var filter = Builders<BsonDocument>.Filter.Eq("date", dateKey);
            var update = Builders<BsonDocument>.Update
                .Inc("totalRevenue", amount)
                .Inc("bookingCount", 1)
                .SetOnInsert("date", dateKey)
                .SetOnInsert("year", date.Year)
                .SetOnInsert("month", date.Month)
                .SetOnInsert("day", date.Day)
                .Set("updatedAt", DateTime.UtcNow);

            await collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating daily revenue");
        }
    }

    private async Task UpdateMonthlyRevenueAsync(DateTime date, decimal amount)
    {
        try
        {
            var collection = _database.GetCollection<BsonDocument>("monthly_revenue");
            var monthKey = date.ToString("yyyy-MM");

            var filter = Builders<BsonDocument>.Filter.Eq("month", monthKey);
            var update = Builders<BsonDocument>.Update
                .Inc("totalRevenue", amount)
                .Inc("bookingCount", 1)
                .SetOnInsert("month", monthKey)
                .SetOnInsert("year", date.Year)
                .SetOnInsert("monthNumber", date.Month)
                .Set("updatedAt", DateTime.UtcNow);

            await collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating monthly revenue");
        }
    }

    private async Task UpdatePaymentMethodStatsAsync(string paymentMethod, decimal amount, DateTime date)
    {
        try
        {
            var collection = _database.GetCollection<BsonDocument>("payment_method_stats");
            var dateKey = date.ToString("yyyy-MM-dd");

            var filter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq("date", dateKey),
                Builders<BsonDocument>.Filter.Eq("paymentMethod", paymentMethod)
            );

            var update = Builders<BsonDocument>.Update
                .Inc("totalAmount", amount)
                .Inc("transactionCount", 1)
                .SetOnInsert("date", dateKey)
                .SetOnInsert("paymentMethod", paymentMethod)
                .Set("updatedAt", DateTime.UtcNow);

            await collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment method stats");
        }
    }
}
