namespace WOL.Shared.Messages.Events;

public class WaitingChargeAccruedEvent
{
    public Guid WaitingChargeId { get; set; }
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid DriverId { get; set; }
    public int HoursCharged { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime Timestamp { get; set; }
}
