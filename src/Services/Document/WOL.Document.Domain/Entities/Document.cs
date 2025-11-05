using WOL.Shared.Common.Domain;
using WOL.Document.Domain.Enums;

namespace WOL.Document.Domain.Entities;

public class Document : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid? VehicleId { get; private set; }
    public DocumentType DocumentType { get; private set; }
    public string DocumentNumber { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public DateTime IssueDate { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public bool IsVerified { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public string? VerifiedBy { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }

    private Document() { }

    public static Document Create(
        Guid userId, 
        DocumentType documentType, 
        string documentNumber, 
        string filePath, 
        DateTime issueDate, 
        DateTime expiryDate,
        Guid? vehicleId = null,
        double? latitude = null,
        double? longitude = null)
    {
        return new Document
        {
            UserId = userId,
            VehicleId = vehicleId,
            DocumentType = documentType,
            DocumentNumber = documentNumber,
            FilePath = filePath,
            IssueDate = issueDate,
            ExpiryDate = expiryDate,
            IsVerified = false,
            Latitude = latitude,
            Longitude = longitude
        };
    }

    public void Verify(string verifiedBy)
    {
        IsVerified = true;
        VerifiedAt = DateTime.UtcNow;
        VerifiedBy = verifiedBy;
        SetUpdatedAt();
    }

    public bool IsExpiringSoon(int daysThreshold = 30)
    {
        return ExpiryDate <= DateTime.UtcNow.AddDays(daysThreshold);
    }

    public bool IsExpired()
    {
        return ExpiryDate < DateTime.UtcNow;
    }
}
