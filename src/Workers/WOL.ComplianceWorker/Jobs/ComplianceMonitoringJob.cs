using Hangfire;
using MassTransit;
using Microsoft.Extensions.Logging;
using WOL.Compliance.Domain.Repositories;
using WOL.Shared.Messages.Events;

namespace WOL.ComplianceWorker.Jobs;

public class ComplianceMonitoringJob
{
    private readonly IComplianceRecordRepository _complianceRecordRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<ComplianceMonitoringJob> _logger;

    public ComplianceMonitoringJob(
        IComplianceRecordRepository complianceRecordRepository,
        IPublishEndpoint publishEndpoint,
        ILogger<ComplianceMonitoringJob> logger)
    {
        _complianceRecordRepository = complianceRecordRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task Execute()
    {
        _logger.LogInformation("Starting compliance monitoring job at {Time}", DateTime.UtcNow);

        try
        {
            var allRecords = await _complianceRecordRepository.GetAllAsync();
            var activeRecords = allRecords.Where(r => r.IsActive).ToList();

            var expiredDocuments = activeRecords
                .Where(r => r.IsExpired())
                .ToList();

            _logger.LogInformation("Found {Count} expired documents", expiredDocuments.Count);

            var groupedByDriver = expiredDocuments
                .Where(d => d.DriverId.HasValue)
                .GroupBy(d => d.DriverId!.Value);

            foreach (var driverGroup in groupedByDriver)
            {
                var driverId = driverGroup.Key;
                var expiredDocs = driverGroup.Select(d => d.DocumentType).ToList();

                await _publishEndpoint.Publish(new ComplianceViolationDetectedEvent
                {
                    DriverId = driverId,
                    VehicleId = null,
                    ViolationType = "ExpiredDocuments",
                    ExpiredDocuments = expiredDocs,
                    Severity = "High",
                    Message = $"Driver has {expiredDocs.Count} expired documents: {string.Join(", ", expiredDocs)}",
                    Timestamp = DateTime.UtcNow
                });

                _logger.LogWarning(
                    "Published ComplianceViolationDetectedEvent for driver {DriverId} with {Count} expired documents",
                    driverId,
                    expiredDocs.Count);
            }

            var groupedByVehicle = expiredDocuments
                .Where(d => d.VehicleId.HasValue)
                .GroupBy(d => d.VehicleId!.Value);

            foreach (var vehicleGroup in groupedByVehicle)
            {
                var vehicleId = vehicleGroup.Key;
                var expiredDocs = vehicleGroup.Select(d => d.DocumentType).ToList();

                await _publishEndpoint.Publish(new ComplianceViolationDetectedEvent
                {
                    DriverId = null,
                    VehicleId = vehicleId,
                    ViolationType = "ExpiredDocuments",
                    ExpiredDocuments = expiredDocs,
                    Severity = "High",
                    Message = $"Vehicle has {expiredDocs.Count} expired documents: {string.Join(", ", expiredDocs)}",
                    Timestamp = DateTime.UtcNow
                });

                _logger.LogWarning(
                    "Published ComplianceViolationDetectedEvent for vehicle {VehicleId} with {Count} expired documents",
                    vehicleId,
                    expiredDocs.Count);
            }

            _logger.LogInformation("Compliance monitoring job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing compliance monitoring job");
            throw;
        }
    }
}
