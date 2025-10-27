namespace WOL.Shared.Messages.Events;

public record BookingCompletedEvent
{
    public Guid BookingId { get; init; }
    public string BookingNumber { get; init; } = string.Empty;
    public Guid CustomerId { get; init; }
    public Guid DriverId { get; init; }
    public decimal TotalFare { get; init; }
    public decimal DriverEarnings { get; init; }
    public DateTime CompletedAt { get; init; }
}
