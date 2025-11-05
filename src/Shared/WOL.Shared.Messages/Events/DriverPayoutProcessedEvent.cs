namespace WOL.Shared.Messages.Events;

public class DriverPayoutProcessedEvent
{
    public Guid PayoutId { get; set; }
    public Guid DriverId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalPenalties { get; set; }
    public decimal NetPayout { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string ProcessedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
