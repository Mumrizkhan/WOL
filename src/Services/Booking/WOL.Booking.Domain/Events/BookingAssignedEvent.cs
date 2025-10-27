using WOL.Shared.Common.Domain;

namespace WOL.Booking.Domain.Events;

public class BookingAssignedEvent : IDomainEvent
{
    public Guid BookingId { get; init; }
    public string BookingNumber { get; init; } = string.Empty;
    public Guid CustomerId { get; init; }
    public Guid VehicleId { get; init; }
    public Guid DriverId { get; init; }
    public DateTime OccurredOn { get; init; }
}
