using MediatR;

namespace WOL.Backload.Application.Commands;

public record CreateBackloadOpportunityCommand : IRequest<CreateBackloadOpportunityResponse>
{
    public Guid VehicleId { get; init; }
    public Guid DriverId { get; init; }
    public string FromCity { get; init; } = string.Empty;
    public string ToCity { get; init; } = string.Empty;
    public DateTime AvailableFrom { get; init; }
    public DateTime AvailableTo { get; init; }
    public decimal AvailableCapacity { get; init; }
}

public record CreateBackloadOpportunityResponse
{
    public Guid OpportunityId { get; init; }
    public string Message { get; init; } = string.Empty;
}
