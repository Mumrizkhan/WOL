namespace WOL.Shared.Messages.Events;

public record BookingAssignedEvent
{
    public Guid BookingId { get; init; }
    public string BookingNumber { get; init; } = string.Empty;
    public Guid CustomerId { get; init; }
    public Guid VehicleId { get; init; }
    public Guid DriverId { get; init; }
    public DateTime AssignedAt { get; init; }
}
