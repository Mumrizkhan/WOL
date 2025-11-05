using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using WOL.Shared.Messages.Events;

namespace WOL.ComplianceWorker;

public class DocumentExpiryMonitorConsumer : IConsumer<DocumentUploadedEvent>
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<DocumentExpiryMonitorConsumer> _logger;

    public DocumentExpiryMonitorConsumer(
        IMongoDatabase mongoDatabase,
        IPublishEndpoint publishEndpoint,
        ILogger<DocumentExpiryMonitorConsumer> logger)
    {
        _mongoDatabase = mongoDatabase;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DocumentUploadedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Checking document expiry for Document {DocumentId} of type {DocumentType}", 
            message.DocumentId, message.DocumentType);

        try
        {
            var expiryDate = message.ExpiryDate;
            var daysUntilExpiry = (expiryDate - DateTime.UtcNow).Days;

            if (daysUntilExpiry < 0)
            {
                await _publishEndpoint.Publish(new DocumentExpiredEvent
                {
                    DocumentId = message.DocumentId,
                    UserId = message.UserId,
                    VehicleId = message.VehicleId,
                    DocumentType = message.DocumentType,
                    DocumentNumber = message.DocumentNumber,
                    ExpiryDate = expiryDate,
                    Timestamp = DateTime.UtcNow
                });

                await _publishEndpoint.Publish(new ComplianceViolationEvent
                {
                    VehicleId = message.VehicleId ?? Guid.Empty,
                    DriverId = null,
                    OwnerId = message.UserId,
                    ViolationType = "ExpiredDocument",
                    Description = $"{message.DocumentType} has expired on {expiryDate:yyyy-MM-dd}",
                    IsBlocked = true,
                    Timestamp = DateTime.UtcNow
                });

                _logger.LogWarning("Document {DocumentId} has EXPIRED. Compliance violation published.", message.DocumentId);
            }
            else if (daysUntilExpiry <= 30)
            {
                await _publishEndpoint.Publish(new DocumentExpiringEvent
                {
                    DocumentId = message.DocumentId,
                    UserId = message.UserId,
                    VehicleId = message.VehicleId,
                    DocumentType = message.DocumentType,
                    DocumentNumber = message.DocumentNumber,
                    ExpiryDate = expiryDate,
                    DaysUntilExpiry = daysUntilExpiry,
                    Timestamp = DateTime.UtcNow
                });

                _logger.LogInformation("Document {DocumentId} is expiring in {Days} days. Notification sent.", 
                    message.DocumentId, daysUntilExpiry);
            }

            var complianceCollection = _mongoDatabase.GetCollection<ComplianceRecord>("compliance_records");
            var complianceRecord = new ComplianceRecord
            {
                Id = Guid.NewGuid().ToString(),
                DocumentId = message.DocumentId,
                UserId = message.UserId,
                VehicleId = message.VehicleId,
                DocumentType = message.DocumentType,
                DocumentNumber = message.DocumentNumber,
                ExpiryDate = expiryDate,
                DaysUntilExpiry = daysUntilExpiry,
                IsExpired = daysUntilExpiry < 0,
                IsExpiringSoon = daysUntilExpiry >= 0 && daysUntilExpiry <= 30,
                CheckedAt = DateTime.UtcNow
            };

            await complianceCollection.InsertOneAsync(complianceRecord);
            _logger.LogInformation("Compliance record created for Document {DocumentId}", message.DocumentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing document expiry check for Document {DocumentId}", message.DocumentId);
            throw;
        }
    }
}

public class DocumentUploadedEvent
{
    public Guid DocumentId { get; set; }
    public Guid UserId { get; set; }
    public Guid? VehicleId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public DateTime Timestamp { get; set; }
}

public class ComplianceRecord
{
    public string Id { get; set; } = string.Empty;
    public Guid DocumentId { get; set; }
    public Guid UserId { get; set; }
    public Guid? VehicleId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int DaysUntilExpiry { get; set; }
    public bool IsExpired { get; set; }
    public bool IsExpiringSoon { get; set; }
    public DateTime CheckedAt { get; set; }
}
