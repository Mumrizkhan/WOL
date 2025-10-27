using WOL.Shared.Common.Domain;

namespace WOL.Payment.Domain.Events;

public class PaymentProcessedEvent : IDomainEvent
{
    public Guid PaymentId { get; init; }
    public Guid BookingId { get; init; }
    public Guid CustomerId { get; init; }
    public decimal Amount { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
    public DateTime OccurredOn { get; init; }
}
