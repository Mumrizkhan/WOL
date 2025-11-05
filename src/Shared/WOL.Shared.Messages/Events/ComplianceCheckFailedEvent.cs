namespace WOL.Shared.Messages.Events;

public class ComplianceCheckFailedEvent
{
    public Guid BookingId { get; set; }
    public Guid DriverId { get; set; }
    public Guid VehicleId { get; set; }
    public List<string> ExpiredDocuments { get; set; } = new();
    public List<string> MissingDocuments { get; set; } = new();
    public string Reason { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
