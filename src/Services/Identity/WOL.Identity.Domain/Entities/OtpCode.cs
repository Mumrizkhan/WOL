using WOL.Identity.Domain.Enums;

namespace WOL.Identity.Domain.Entities;

public class OtpCode
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public OtpPurpose Purpose { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int AttemptCount { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;

    public bool IsValid()
    {
        return !IsUsed && DateTime.UtcNow < ExpiresAt && AttemptCount < 5;
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
        UsedAt = DateTime.UtcNow;
    }

    public void IncrementAttempt()
    {
        AttemptCount++;
    }
}
