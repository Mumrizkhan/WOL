namespace WOL.Shared.Messages.Events;

public record PaymentProcessedEvent
{
    public Guid PaymentId { get; init; }
    public Guid BookingId { get; init; }
    public Guid CustomerId { get; init; }
    public decimal Amount { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
    public string TransactionId { get; init; } = string.Empty;
    public DateTime ProcessedAt { get; init; }
}
