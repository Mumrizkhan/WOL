namespace WOL.Shared.Messages.Events;

public class SurgePricingAppliedEvent
{
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public string City { get; set; } = string.Empty;
    public decimal BaseAmount { get; set; }
    public decimal Multiplier { get; set; }
    public decimal SurgeAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public DateTime Timestamp { get; set; }
}
