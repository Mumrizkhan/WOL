namespace WOL.Shared.Messages.Events;

public class RouteDeviationDetectedEvent
{
    public Guid BookingId { get; set; }
    public Guid DriverId { get; set; }
    public Guid VehicleId { get; set; }
    public double CurrentLatitude { get; set; }
    public double CurrentLongitude { get; set; }
    public double DeviationDistanceKm { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime DetectedAt { get; set; }
    public DateTime Timestamp { get; set; }
}
