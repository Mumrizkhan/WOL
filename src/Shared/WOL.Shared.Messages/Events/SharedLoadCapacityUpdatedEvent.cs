namespace WOL.Shared.Messages.Events;

public class SharedLoadCapacityUpdatedEvent
{
    public Guid SharedLoadBookingId { get; set; }
    public Guid VehicleId { get; set; }
    public Guid DriverId { get; set; }
    public string OriginCity { get; set; } = string.Empty;
    public string DestinationCity { get; set; } = string.Empty;
    public decimal TotalCapacityKg { get; set; }
    public decimal UsedCapacityKg { get; set; }
    public decimal AvailableCapacityKg { get; set; }
    public decimal CapacityUtilization { get; set; }
    public int BookingCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
