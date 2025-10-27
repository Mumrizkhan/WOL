using WOL.Shared.Common.Domain;

namespace WOL.Tracking.Domain.Entities;

public class LocationHistory : BaseEntity
{
    public Guid BookingId { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid DriverId { get; private set; }
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }
    public decimal? Speed { get; private set; }
    public decimal? Heading { get; private set; }
    public DateTime Timestamp { get; private set; }

    private LocationHistory() { }

    public static LocationHistory Create(
        Guid bookingId,
        Guid vehicleId,
        Guid driverId,
        decimal latitude,
        decimal longitude,
        decimal? speed,
        decimal? heading)
    {
        return new LocationHistory
        {
            BookingId = bookingId,
            VehicleId = vehicleId,
            DriverId = driverId,
            Latitude = latitude,
            Longitude = longitude,
            Speed = speed,
            Heading = heading,
            Timestamp = DateTime.UtcNow
        };
    }
}
