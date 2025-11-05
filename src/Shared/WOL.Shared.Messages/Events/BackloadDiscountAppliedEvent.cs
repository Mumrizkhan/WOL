namespace WOL.Shared.Messages.Events;

public class BackloadDiscountAppliedEvent
{
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? BackloadOpportunityId { get; set; }
    public decimal OriginalFare { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalFare { get; set; }
    public DateTime Timestamp { get; set; }
}
