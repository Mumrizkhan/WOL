using WOL.Shared.Common.Domain;
using WOL.Booking.Domain.Enums;

namespace WOL.Booking.Domain.Entities;

public class CancellationFee : BaseEntity
{
    public Guid BookingId { get; private set; }
    public CancellationReason Reason { get; private set; }
    public CancellationParty CancelledBy { get; private set; }
    public CancellationParty ChargedTo { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime CancelledAt { get; private set; }
    public bool IsPaid { get; private set; }
    public string? Notes { get; private set; }

    private CancellationFee() { }

    public static CancellationFee Create(
        Guid bookingId,
        CancellationReason reason,
        CancellationParty cancelledBy,
        CancellationParty chargedTo,
        decimal amount,
        string? notes = null)
    {
        return new CancellationFee
        {
            BookingId = bookingId,
            Reason = reason,
            CancelledBy = cancelledBy,
            ChargedTo = chargedTo,
            Amount = amount,
            CancelledAt = DateTime.UtcNow,
            IsPaid = false,
            Notes = notes
        };
    }

    public void MarkPaid()
    {
        IsPaid = true;
        SetUpdatedAt();
    }
}
