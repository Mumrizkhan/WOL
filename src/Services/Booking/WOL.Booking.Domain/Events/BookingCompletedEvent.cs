using WOL.Shared.Common.Domain;

namespace WOL.Booking.Domain.Events;

public class BookingCompletedEvent : IDomainEvent
{
    public Guid BookingId { get; init; }
    public string BookingNumber { get; init; } = string.Empty;
    public Guid CustomerId { get; init; }
    public Guid DriverId { get; init; }
    public decimal TotalFare { get; init; }
    public DateTime OccurredOn { get; init; }
}
