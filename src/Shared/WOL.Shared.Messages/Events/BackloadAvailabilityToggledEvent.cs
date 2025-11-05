namespace WOL.Shared.Messages.Events;

public class BackloadAvailabilityToggledEvent
{
    public Guid DriverId { get; set; }
    public Guid VehicleId { get; set; }
    public bool IsAvailable { get; set; }
    public string OriginCity { get; set; } = string.Empty;
    public string DestinationCity { get; set; } = string.Empty;
    public DateTime AvailableFrom { get; set; }
    public DateTime AvailableTo { get; set; }
    public DateTime Timestamp { get; set; }
}
