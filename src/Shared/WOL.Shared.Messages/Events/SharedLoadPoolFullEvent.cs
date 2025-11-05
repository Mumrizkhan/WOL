namespace WOL.Shared.Messages.Events;

public class SharedLoadPoolFullEvent
{
    public Guid SharedLoadId { get; set; }
    public string OriginCity { get; set; } = string.Empty;
    public string DestinationCity { get; set; } = string.Empty;
    public int TotalBookings { get; set; }
    public decimal TotalWeight { get; set; }
    public decimal TotalVolume { get; set; }
    public DateTime Timestamp { get; set; }
}
