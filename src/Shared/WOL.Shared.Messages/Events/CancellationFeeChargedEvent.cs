namespace WOL.Shared.Messages.Events;

public class CancellationFeeChargedEvent
{
    public Guid CancellationFeeId { get; set; }
    public Guid BookingId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string CancelledBy { get; set; } = string.Empty;
    public string ChargedTo { get; set; } = string.Empty;
    public Guid ChargedToUserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CancelledAt { get; set; }
    public string? Notes { get; set; }
    public DateTime Timestamp { get; set; }
}
