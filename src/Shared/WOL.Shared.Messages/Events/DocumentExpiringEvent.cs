namespace WOL.Shared.Messages.Events;

public record DocumentExpiringEvent
{
    public Guid DocumentId { get; init; }
    public Guid EntityId { get; init; }
    public string EntityType { get; init; } = string.Empty;
    public string DocumentType { get; init; } = string.Empty;
    public DateTime ExpiryDate { get; init; }
    public int DaysUntilExpiry { get; init; }
}
