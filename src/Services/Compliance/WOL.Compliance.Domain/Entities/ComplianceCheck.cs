using WOL.Shared.Common.Domain;

namespace WOL.Compliance.Domain.Entities;

public class ComplianceCheck : BaseEntity
{
    public Guid EntityId { get; private set; }
    public string EntityType { get; private set; } = string.Empty;
    public string CheckType { get; private set; } = string.Empty;
    public string Status { get; private set; } = "Pending";
    public string? Result { get; private set; }
    public DateTime? CheckedAt { get; private set; }
    public string? Notes { get; private set; }

    private ComplianceCheck() { }

    public static ComplianceCheck Create(Guid entityId, string entityType, string checkType)
    {
        return new ComplianceCheck
        {
            EntityId = entityId,
            EntityType = entityType,
            CheckType = checkType,
            Status = "Pending"
        };
    }

    public void MarkAsCompliant(string notes)
    {
        Status = "Compliant";
        Result = "Pass";
        CheckedAt = DateTime.UtcNow;
        Notes = notes;
        SetUpdatedAt();
    }

    public void MarkAsNonCompliant(string notes)
    {
        Status = "NonCompliant";
        Result = "Fail";
        CheckedAt = DateTime.UtcNow;
        Notes = notes;
        SetUpdatedAt();
    }
}
