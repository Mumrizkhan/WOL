namespace WOL.Shared.Messages.Events;

public class AutoRefundInitiatedEvent
{
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid PaymentId { get; set; }
    public decimal RefundAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime InitiatedAt { get; set; }
    public DateTime Timestamp { get; set; }
}
