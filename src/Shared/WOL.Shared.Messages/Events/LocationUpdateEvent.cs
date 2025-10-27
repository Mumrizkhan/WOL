namespace WOL.Shared.Messages.Events;

public record LocationUpdateEvent
{
    public Guid VehicleId { get; init; }
    public Guid? BookingId { get; init; }
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
    public decimal? Speed { get; init; }
    public decimal? Heading { get; init; }
    public DateTime Timestamp { get; init; }
}
