namespace WOL.Shared.Messages.Events;

public class LocationUpdatedEvent
{
    public Guid BookingId { get; set; }
    public Guid DriverId { get; set; }
    public Guid VehicleId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Speed { get; set; }
    public double Heading { get; set; }
    public DateTime Timestamp { get; set; }
}
