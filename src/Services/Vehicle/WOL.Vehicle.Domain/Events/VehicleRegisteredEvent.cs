using WOL.Shared.Common.Domain;

namespace WOL.Vehicle.Domain.Events;

public class VehicleRegisteredEvent : IDomainEvent
{
    public Guid VehicleId { get; init; }
    public Guid OwnerId { get; init; }
    public string PlateNumber { get; init; } = string.Empty;
    public DateTime OccurredOn { get; init; }
}
