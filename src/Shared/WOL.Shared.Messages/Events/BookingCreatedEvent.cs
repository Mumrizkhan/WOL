namespace WOL.Shared.Messages.Events;

public record BookingCreatedEvent
{
    public Guid BookingId { get; init; }
    public string BookingNumber { get; init; } = string.Empty;
    public Guid CustomerId { get; init; }
    public Guid VehicleTypeId { get; init; }
    public string OriginAddress { get; init; } = string.Empty;
    public string DestinationAddress { get; init; } = string.Empty;
    public DateTime PickupDate { get; init; }
    public TimeSpan PickupTime { get; init; }
    public decimal TotalFare { get; init; }
    public DateTime CreatedAt { get; init; }
}
