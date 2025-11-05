using Hangfire;
using MassTransit;
using Microsoft.Extensions.Logging;
using WOL.Compliance.Domain.Repositories;
using WOL.Shared.Messages.Events;

namespace WOL.ComplianceWorker.Jobs;

public class DocumentExpiryCheckJob
{
    private readonly IComplianceRecordRepository _complianceRecordRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<DocumentExpiryCheckJob> _logger;

    public DocumentExpiryCheckJob(
        IComplianceRecordRepository complianceRecordRepository,
        IPublishEndpoint publishEndpoint,
        ILogger<DocumentExpiryCheckJob> logger)
    {
        _complianceRecordRepository = complianceRecordRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task Execute()
    {
        _logger.LogInformation("Starting document expiry check job at {Time}", DateTime.UtcNow);

        try
        {
            var allRecords = await _complianceRecordRepository.GetAllAsync();
            var activeRecords = allRecords.Where(r => r.IsActive).ToList();

            var expiringDocuments = activeRecords
                .Where(r => r.IsExpiringSoon(30))
                .ToList();

            _logger.LogInformation("Found {Count} documents expiring within 30 days", expiringDocuments.Count);

            foreach (var document in expiringDocuments)
            {
                await _publishEndpoint.Publish(new DocumentExpiringEvent
                {
                    DocumentId = document.Id,
                    DriverId = document.DriverId,
                    VehicleId = document.VehicleId,
                    DocumentType = document.DocumentType,
                    ExpiryDate = document.ExpiryDate,
                    DaysUntilExpiry = (document.ExpiryDate - DateTime.UtcNow).Days,
                    Timestamp = DateTime.UtcNow
                });

                _logger.LogInformation(
                    "Published DocumentExpiringEvent for {DocumentType} expiring on {ExpiryDate}",
                    document.DocumentType,
                    document.ExpiryDate);
            }

            _logger.LogInformation("Document expiry check job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing document expiry check job");
            throw;
        }
    }
}
