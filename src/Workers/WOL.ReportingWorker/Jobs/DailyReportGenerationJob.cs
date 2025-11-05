using Hangfire;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace WOL.ReportingWorker.Jobs;

public class DailyReportGenerationJob
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly ILogger<DailyReportGenerationJob> _logger;

    public DailyReportGenerationJob(
        IMongoDatabase mongoDatabase,
        ILogger<DailyReportGenerationJob> logger)
    {
        _mongoDatabase = mongoDatabase;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task Execute()
    {
        _logger.LogInformation("Starting daily report generation job at {Time}", DateTime.UtcNow);

        try
        {
            var yesterday = DateTime.UtcNow.Date.AddDays(-1);
            var today = DateTime.UtcNow.Date;

            var bookingAnalytics = _mongoDatabase.GetCollection<dynamic>("booking_analytics");
            var paymentAnalytics = _mongoDatabase.GetCollection<dynamic>("payment_analytics");
            var complianceHistory = _mongoDatabase.GetCollection<dynamic>("compliance_history");

            var totalBookings = await bookingAnalytics
                .CountDocumentsAsync(Builders<dynamic>.Filter.And(
                    Builders<dynamic>.Filter.Gte("timestamp", yesterday),
                    Builders<dynamic>.Filter.Lt("timestamp", today)));

            var totalRevenue = await paymentAnalytics
                .Aggregate()
                .Match(Builders<dynamic>.Filter.And(
                    Builders<dynamic>.Filter.Gte("timestamp", yesterday),
                    Builders<dynamic>.Filter.Lt("timestamp", today),
                    Builders<dynamic>.Filter.Eq("status", "Completed")))
                .Group(new BsonDocument
                {
                    { "_id", BsonNull.Value },
                    { "total", new BsonDocument("$sum", "$amount") }
                })
                .FirstOrDefaultAsync();

            var complianceViolations = await complianceHistory
                .CountDocumentsAsync(Builders<dynamic>.Filter.And(
                    Builders<dynamic>.Filter.Gte("timestamp", yesterday),
                    Builders<dynamic>.Filter.Lt("timestamp", today),
                    Builders<dynamic>.Filter.Eq("status", "Violated")));

            var reportData = new
            {
                ReportDate = yesterday,
                GeneratedAt = DateTime.UtcNow,
                TotalBookings = totalBookings,
                TotalRevenue = totalRevenue?["total"]?.AsDecimal ?? 0,
                ComplianceViolations = complianceViolations,
                ReportType = "Daily"
            };

            var dailyReports = _mongoDatabase.GetCollection<dynamic>("daily_reports");
            await dailyReports.InsertOneAsync(reportData);

            _logger.LogInformation(
                "Daily report generated: {Bookings} bookings, {Revenue} SAR revenue, {Violations} compliance violations",
                totalBookings,
                reportData.TotalRevenue,
                complianceViolations);

            _logger.LogInformation("Daily report generation job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing daily report generation job");
            throw;
        }
    }
}
