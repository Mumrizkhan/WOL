namespace WOL.Shared.Messages.Events;

public class BANViolationAttemptedEvent
{
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public string City { get; set; } = string.Empty;
    public DateTime AttemptedPickupTime { get; set; }
    public TimeSpan BANStartTime { get; set; }
    public TimeSpan BANEndTime { get; set; }
    public DateTime Timestamp { get; set; }
}
