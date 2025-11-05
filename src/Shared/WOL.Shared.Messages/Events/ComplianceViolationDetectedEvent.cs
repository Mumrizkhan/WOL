namespace WOL.Shared.Messages.Events;

public class ComplianceViolationDetectedEvent
{
    public Guid? DriverId { get; set; }
    public Guid? VehicleId { get; set; }
    public string ViolationType { get; set; } = string.Empty;
    public List<string> ExpiredDocuments { get; set; } = new();
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
