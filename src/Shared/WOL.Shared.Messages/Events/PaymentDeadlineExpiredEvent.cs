namespace WOL.Shared.Messages.Events;

public class PaymentDeadlineExpiredEvent
{
    public Guid PaymentDeadlineId { get; set; }
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime DeadlineAt { get; set; }
    public DateTime ExpiredAt { get; set; }
    public DateTime Timestamp { get; set; }
}
