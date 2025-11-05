namespace WOL.Shared.Messages.Events;

public class ComplianceViolationEvent
{
    public Guid VehicleId { get; set; }
    public Guid? DriverId { get; set; }
    public Guid OwnerId { get; set; }
    public string ViolationType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsBlocked { get; set; }
    public DateTime Timestamp { get; set; }
}
