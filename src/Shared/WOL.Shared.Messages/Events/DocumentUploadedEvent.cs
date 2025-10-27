namespace WOL.Shared.Messages.Events;

public record DocumentUploadedEvent
{
    public Guid DocumentId { get; init; }
    public Guid EntityId { get; init; }
    public string EntityType { get; init; } = string.Empty;
    public string DocumentType { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public string FileUrl { get; init; } = string.Empty;
    public DateTime UploadedAt { get; init; }
}
