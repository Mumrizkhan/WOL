namespace WOL.Shared.Messages.Events;

public record VehicleRegisteredEvent
{
    public Guid VehicleId { get; init; }
    public Guid OwnerId { get; init; }
    public string PlateNumber { get; init; } = string.Empty;
    public Guid VehicleTypeId { get; init; }
    public DateTime RegisteredAt { get; init; }
}
