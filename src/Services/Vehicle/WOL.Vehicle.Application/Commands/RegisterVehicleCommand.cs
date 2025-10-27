using MediatR;

namespace WOL.Vehicle.Application.Commands;

public record RegisterVehicleCommand : IRequest<RegisterVehicleResponse>
{
    public Guid OwnerId { get; init; }
    public Guid VehicleTypeId { get; init; }
    public string PlateNumber { get; init; } = string.Empty;
    public string Make { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public int Year { get; init; }
    public string Color { get; init; } = string.Empty;
}

public record RegisterVehicleResponse
{
    public Guid VehicleId { get; init; }
    public string PlateNumber { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}
