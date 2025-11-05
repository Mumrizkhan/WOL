using WOL.Shared.Common.Domain;

namespace WOL.Booking.Domain.Events;

public class BookingCancelledEvent : DomainEvent
{
    public Guid BookingId { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public Guid? DriverId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CancelledAt { get; set; }
}
