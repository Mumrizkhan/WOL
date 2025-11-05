namespace WOL.Shared.Messages.Events;

public class RouteUtilizationUpdatedEvent
{
    public Guid RouteUtilizationId { get; set; }
    public string OriginCity { get; set; } = string.Empty;
    public string DestinationCity { get; set; } = string.Empty;
    public int OutboundBookings { get; set; }
    public int ReturnBookings { get; set; }
    public decimal UtilizationPercentage { get; set; }
    public decimal EmptyKmTotal { get; set; }
    public decimal EmptyKmSaved { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime Timestamp { get; set; }
}
