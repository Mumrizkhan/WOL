namespace WOL.ComplianceWorker.Services;

public interface IComplianceService
{
    Task CheckVehicleComplianceAsync(Guid vehicleId, string registrationNumber, DateTime registrationExpiry, DateTime insuranceExpiry);
    Task CheckDocumentComplianceAsync(Guid documentId, string documentType, DateTime expiryDate);
}
