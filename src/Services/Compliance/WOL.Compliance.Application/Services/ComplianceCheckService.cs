using WOL.Compliance.Domain.Repositories;

namespace WOL.Compliance.Application.Services;

public class ComplianceCheckService
{
    private readonly IComplianceRecordRepository _complianceRecordRepository;

    public ComplianceCheckService(IComplianceRecordRepository complianceRecordRepository)
    {
        _complianceRecordRepository = complianceRecordRepository;
    }

    public async Task<ComplianceCheckResult> CheckDriverComplianceAsync(Guid driverId)
    {
        var records = await _complianceRecordRepository.GetByDriverIdAsync(driverId);
        var activeRecords = records.Where(r => r.IsActive).ToList();

        var expiredDocuments = new List<string>();
        var expiringDocuments = new List<string>();

        foreach (var record in activeRecords)
        {
            if (record.IsExpired())
            {
                expiredDocuments.Add(record.DocumentType);
            }
            else if (record.IsExpiringSoon(30))
            {
                expiringDocuments.Add(record.DocumentType);
            }
        }

        var requiredDocuments = new[] { "DriverLicense", "Iqama" };
        var missingDocuments = requiredDocuments
            .Where(doc => !activeRecords.Any(r => r.DocumentType == doc))
            .ToList();

        var isCompliant = expiredDocuments.Count == 0 && missingDocuments.Count == 0;

        return new ComplianceCheckResult
        {
            IsCompliant = isCompliant,
            ExpiredDocuments = expiredDocuments,
            ExpiringDocuments = expiringDocuments,
            MissingDocuments = missingDocuments,
            Message = isCompliant 
                ? "Driver is compliant" 
                : $"Driver has {expiredDocuments.Count} expired, {expiringDocuments.Count} expiring, and {missingDocuments.Count} missing documents"
        };
    }

    public async Task<ComplianceCheckResult> CheckVehicleComplianceAsync(Guid vehicleId)
    {
        var records = await _complianceRecordRepository.GetByVehicleIdAsync(vehicleId);
        var activeRecords = records.Where(r => r.IsActive).ToList();

        var expiredDocuments = new List<string>();
        var expiringDocuments = new List<string>();

        foreach (var record in activeRecords)
        {
            if (record.IsExpired())
            {
                expiredDocuments.Add(record.DocumentType);
            }
            else if (record.IsExpiringSoon(30))
            {
                expiringDocuments.Add(record.DocumentType);
            }
        }

        var requiredDocuments = new[] { "Istemara", "MVPI", "Insurance" };
        var missingDocuments = requiredDocuments
            .Where(doc => !activeRecords.Any(r => r.DocumentType == doc))
            .ToList();

        var isCompliant = expiredDocuments.Count == 0 && missingDocuments.Count == 0;

        return new ComplianceCheckResult
        {
            IsCompliant = isCompliant,
            ExpiredDocuments = expiredDocuments,
            ExpiringDocuments = expiringDocuments,
            MissingDocuments = missingDocuments,
            Message = isCompliant 
                ? "Vehicle is compliant" 
                : $"Vehicle has {expiredDocuments.Count} expired, {expiringDocuments.Count} expiring, and {missingDocuments.Count} missing documents"
        };
    }

    public async Task<ComplianceCheckResult> CheckBookingComplianceAsync(Guid driverId, Guid vehicleId)
    {
        var driverCompliance = await CheckDriverComplianceAsync(driverId);
        var vehicleCompliance = await CheckVehicleComplianceAsync(vehicleId);

        var allExpired = driverCompliance.ExpiredDocuments.Concat(vehicleCompliance.ExpiredDocuments).ToList();
        var allExpiring = driverCompliance.ExpiringDocuments.Concat(vehicleCompliance.ExpiringDocuments).ToList();
        var allMissing = driverCompliance.MissingDocuments.Concat(vehicleCompliance.MissingDocuments).ToList();

        var isCompliant = driverCompliance.IsCompliant && vehicleCompliance.IsCompliant;

        return new ComplianceCheckResult
        {
            IsCompliant = isCompliant,
            ExpiredDocuments = allExpired,
            ExpiringDocuments = allExpiring,
            MissingDocuments = allMissing,
            Message = isCompliant 
                ? "Booking is compliant for dispatch" 
                : $"Booking cannot be dispatched: {allExpired.Count} expired, {allMissing.Count} missing documents"
        };
    }
}

public class ComplianceCheckResult
{
    public bool IsCompliant { get; set; }
    public List<string> ExpiredDocuments { get; set; } = new();
    public List<string> ExpiringDocuments { get; set; } = new();
    public List<string> MissingDocuments { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}
