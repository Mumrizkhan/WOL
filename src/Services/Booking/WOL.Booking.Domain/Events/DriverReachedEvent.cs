using WOL.Shared.Common.Domain;

namespace WOL.Booking.Domain.Events;

public class DriverReachedEvent : DomainEvent
{
    public Guid BookingId { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public Guid DriverId { get; set; }
    public Guid VehicleId { get; set; }
    public DateTime ReachedAt { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? PhotoPath { get; set; }
}
