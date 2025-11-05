namespace WOL.Shared.Messages.Events;

public class ETAUpdatedEvent
{
    public Guid BookingId { get; set; }
    public Guid DriverId { get; set; }
    public Guid CustomerId { get; set; }
    public double CurrentLatitude { get; set; }
    public double CurrentLongitude { get; set; }
    public double RemainingDistanceKm { get; set; }
    public DateTime EstimatedArrival { get; set; }
    public TimeSpan RemainingTime { get; set; }
    public DateTime Timestamp { get; set; }
}
