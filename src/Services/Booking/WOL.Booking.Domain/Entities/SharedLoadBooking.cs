using WOL.Shared.Common.Domain;

namespace WOL.Booking.Domain.Entities;

public class SharedLoadBooking : BaseEntity
{
    public Guid VehicleId { get; private set; }
    public Guid DriverId { get; private set; }
    public DateTime PickupDate { get; private set; }
    public string OriginCity { get; private set; } = string.Empty;
    public string DestinationCity { get; private set; } = string.Empty;
    public decimal TotalCapacityKg { get; private set; }
    public decimal UsedCapacityKg { get; private set; }
    public decimal AvailableCapacityKg { get; private set; }
    public string Status { get; private set; } = "Open";

    private readonly List<Guid> _bookingIds = new();
    public IReadOnlyCollection<Guid> BookingIds => _bookingIds.AsReadOnly();

    private SharedLoadBooking() { }

    public static SharedLoadBooking Create(
        Guid vehicleId,
        Guid driverId,
        DateTime pickupDate,
        string originCity,
        string destinationCity,
        decimal totalCapacityKg)
    {
        return new SharedLoadBooking
        {
            VehicleId = vehicleId,
            DriverId = driverId,
            PickupDate = pickupDate,
            OriginCity = originCity,
            DestinationCity = destinationCity,
            TotalCapacityKg = totalCapacityKg,
            UsedCapacityKg = 0,
            AvailableCapacityKg = totalCapacityKg,
            Status = "Open"
        };
    }

    public bool CanAccommodate(decimal weightKg)
    {
        return Status == "Open" && AvailableCapacityKg >= weightKg;
    }

    public void AddBooking(Guid bookingId, decimal weightKg)
    {
        if (!CanAccommodate(weightKg))
            throw new InvalidOperationException("Insufficient capacity for this booking");

        _bookingIds.Add(bookingId);
        UsedCapacityKg += weightKg;
        AvailableCapacityKg = TotalCapacityKg - UsedCapacityKg;

        if (AvailableCapacityKg < (TotalCapacityKg * 0.1m))
        {
            Status = "Full";
        }

        SetUpdatedAt();
    }

    public void RemoveBooking(Guid bookingId, decimal weightKg)
    {
        _bookingIds.Remove(bookingId);
        UsedCapacityKg -= weightKg;
        AvailableCapacityKg = TotalCapacityKg - UsedCapacityKg;
        
        if (Status == "Full" && AvailableCapacityKg >= (TotalCapacityKg * 0.1m))
        {
            Status = "Open";
        }

        SetUpdatedAt();
    }

    public decimal GetCapacityUtilization()
    {
        return (UsedCapacityKg / TotalCapacityKg) * 100;
    }

    public void Close()
    {
        Status = "Closed";
        SetUpdatedAt();
    }
}
