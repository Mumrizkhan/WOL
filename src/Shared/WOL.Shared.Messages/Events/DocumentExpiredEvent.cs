namespace WOL.Shared.Messages.Events;

public class DocumentExpiredEvent
{
    public Guid DocumentId { get; set; }
    public Guid UserId { get; set; }
    public Guid? VehicleId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public DateTime Timestamp { get; set; }
}
