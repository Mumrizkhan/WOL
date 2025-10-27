using WOL.Shared.Common.Domain;

namespace WOL.Document.Domain.Entities;

public class Document : BaseEntity
{
    public Guid UserId { get; private set; }
    public string DocumentType { get; private set; } = string.Empty;
    public string DocumentNumber { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public DateTime IssueDate { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public bool IsVerified { get; private set; }
    public DateTime? VerifiedAt { get; private set; }

    private Document() { }

    public static Document Create(Guid userId, string documentType, string documentNumber, string filePath, DateTime issueDate, DateTime expiryDate)
    {
        return new Document
        {
            UserId = userId,
            DocumentType = documentType,
            DocumentNumber = documentNumber,
            FilePath = filePath,
            IssueDate = issueDate,
            ExpiryDate = expiryDate,
            IsVerified = false
        };
    }

    public void Verify()
    {
        IsVerified = true;
        VerifiedAt = DateTime.UtcNow;
        SetUpdatedAt();
    }
}
