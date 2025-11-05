namespace WOL.Shared.Messages.Events;

public class LoyaltyDiscountAppliedEvent
{
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public string TierLevel { get; set; } = string.Empty;
    public int TotalBookings { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal OriginalAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public DateTime Timestamp { get; set; }
}
