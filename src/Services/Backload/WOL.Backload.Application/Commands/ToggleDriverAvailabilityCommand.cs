using MediatR;
using MassTransit;
using WOL.Backload.Domain.Entities;
using WOL.Backload.Domain.Repositories;
using WOL.Shared.Messages.Events;

namespace WOL.Backload.Application.Commands;

public record ToggleDriverAvailabilityCommand : IRequest<ToggleDriverAvailabilityResponse>
{
    public Guid DriverId { get; init; }
    public Guid VehicleId { get; init; }
    public bool IsAvailable { get; init; }
    public string OriginCity { get; init; } = string.Empty;
    public string DestinationCity { get; init; } = string.Empty;
    public DateTime AvailableFrom { get; init; }
    public DateTime AvailableTo { get; init; }
    public decimal VehicleCapacity { get; init; }
    public string VehicleType { get; init; } = string.Empty;
}

public record ToggleDriverAvailabilityResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public Guid? OpportunityId { get; init; }
}

public class ToggleDriverAvailabilityCommandHandler : IRequestHandler<ToggleDriverAvailabilityCommand, ToggleDriverAvailabilityResponse>
{
    private readonly IBackloadOpportunityRepository _opportunityRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ToggleDriverAvailabilityCommandHandler(
        IBackloadOpportunityRepository opportunityRepository,
        IPublishEndpoint publishEndpoint)
    {
        _opportunityRepository = opportunityRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<ToggleDriverAvailabilityResponse> Handle(ToggleDriverAvailabilityCommand request, CancellationToken cancellationToken)
    {
        if (request.IsAvailable)
        {
            var opportunity = BackloadOpportunity.Create(
                request.DriverId,
                request.VehicleId,
                request.OriginCity,
                request.DestinationCity,
                request.AvailableFrom,
                request.AvailableTo,
                request.VehicleCapacity,
                request.VehicleType);

            await _opportunityRepository.AddAsync(opportunity);

            await _publishEndpoint.Publish(new BackloadAvailabilityToggledEvent
            {
                DriverId = request.DriverId,
                VehicleId = request.VehicleId,
                IsAvailable = true,
                OriginCity = request.OriginCity,
                DestinationCity = request.DestinationCity,
                AvailableFrom = request.AvailableFrom,
                AvailableTo = request.AvailableTo,
                Timestamp = DateTime.UtcNow
            }, cancellationToken);

            return new ToggleDriverAvailabilityResponse
            {
                Success = true,
                Message = "Driver availability enabled. Backload opportunity created.",
                OpportunityId = opportunity.Id
            };
        }
        else
        {
            var opportunities = await _opportunityRepository.GetByDriverIdAsync(request.DriverId);
            var activeOpportunity = opportunities.FirstOrDefault(o => o.Status == "Available");

            if (activeOpportunity != null)
            {
                activeOpportunity.MarkAsUnavailable();
                await _opportunityRepository.UpdateAsync(activeOpportunity);
            }

            await _publishEndpoint.Publish(new BackloadAvailabilityToggledEvent
            {
                DriverId = request.DriverId,
                VehicleId = request.VehicleId,
                IsAvailable = false,
                OriginCity = string.Empty,
                DestinationCity = string.Empty,
                AvailableFrom = DateTime.UtcNow,
                AvailableTo = DateTime.UtcNow,
                Timestamp = DateTime.UtcNow
            }, cancellationToken);

            return new ToggleDriverAvailabilityResponse
            {
                Success = true,
                Message = "Driver availability disabled. Backload opportunity closed.",
                OpportunityId = activeOpportunity?.Id
            };
        }
    }
}
