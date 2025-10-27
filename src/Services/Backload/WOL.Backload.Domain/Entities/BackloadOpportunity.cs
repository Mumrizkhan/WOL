using WOL.Shared.Common.Domain;

namespace WOL.Backload.Domain.Entities;

public class BackloadOpportunity : BaseEntity
{
    public Guid VehicleId { get; private set; }
    public Guid DriverId { get; private set; }
    public string FromCity { get; private set; } = string.Empty;
    public string ToCity { get; private set; } = string.Empty;
    public DateTime AvailableFrom { get; private set; }
    public DateTime AvailableTo { get; private set; }
    public decimal AvailableCapacity { get; private set; }
    public string Status { get; private set; } = "Available";
    public Guid? MatchedBookingId { get; private set; }

    private BackloadOpportunity() { }

    public static BackloadOpportunity Create(Guid vehicleId, Guid driverId, string fromCity, string toCity, 
        DateTime availableFrom, DateTime availableTo, decimal availableCapacity)
    {
        return new BackloadOpportunity
        {
            VehicleId = vehicleId,
            DriverId = driverId,
            FromCity = fromCity,
            ToCity = toCity,
            AvailableFrom = availableFrom,
            AvailableTo = availableTo,
            AvailableCapacity = availableCapacity,
            Status = "Available"
        };
    }

    public void MatchWithBooking(Guid bookingId)
    {
        MatchedBookingId = bookingId;
        Status = "Matched";
        SetUpdatedAt();
    }

    public void Complete()
    {
        Status = "Completed";
        SetUpdatedAt();
    }
}
