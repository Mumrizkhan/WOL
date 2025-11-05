using MediatR;
using WOL.Backload.Domain.Repositories;

namespace WOL.Backload.Application.Queries;

public record GetRouteUtilizationQuery : IRequest<GetRouteUtilizationResponse>
{
    public string? OriginCity { get; init; }
    public string? DestinationCity { get; init; }
}

public record GetRouteUtilizationResponse
{
    public List<RouteUtilizationDto> Routes { get; init; } = new();
}

public record RouteUtilizationDto
{
    public Guid Id { get; init; }
    public string OriginCity { get; init; } = string.Empty;
    public string DestinationCity { get; init; } = string.Empty;
    public int OutboundBookings { get; init; }
    public int ReturnBookings { get; init; }
    public decimal UtilizationPercentage { get; init; }
    public decimal EmptyKmTotal { get; init; }
    public decimal EmptyKmSaved { get; init; }
    public DateTime PeriodStart { get; init; }
    public DateTime PeriodEnd { get; init; }
}

public class GetRouteUtilizationQueryHandler : IRequestHandler<GetRouteUtilizationQuery, GetRouteUtilizationResponse>
{
    private readonly IRouteUtilizationRepository _routeUtilizationRepository;

    public GetRouteUtilizationQueryHandler(IRouteUtilizationRepository routeUtilizationRepository)
    {
        _routeUtilizationRepository = routeUtilizationRepository;
    }

    public async Task<GetRouteUtilizationResponse> Handle(GetRouteUtilizationQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<WOL.Backload.Domain.Entities.RouteUtilization> routes;

        if (!string.IsNullOrEmpty(request.OriginCity) && !string.IsNullOrEmpty(request.DestinationCity))
        {
            var route = await _routeUtilizationRepository.GetByRouteAsync(request.OriginCity, request.DestinationCity);
            routes = route != null ? new[] { route } : Array.Empty<WOL.Backload.Domain.Entities.RouteUtilization>();
        }
        else if (!string.IsNullOrEmpty(request.OriginCity))
        {
            routes = await _routeUtilizationRepository.GetByOriginCityAsync(request.OriginCity);
        }
        else if (!string.IsNullOrEmpty(request.DestinationCity))
        {
            routes = await _routeUtilizationRepository.GetByDestinationCityAsync(request.DestinationCity);
        }
        else
        {
            routes = await _routeUtilizationRepository.GetAllAsync();
        }

        var dtos = routes.Select(r => new RouteUtilizationDto
        {
            Id = r.Id,
            OriginCity = r.OriginCity,
            DestinationCity = r.DestinationCity,
            OutboundBookings = r.OutboundBookings,
            ReturnBookings = r.ReturnBookings,
            UtilizationPercentage = r.UtilizationPercentage,
            EmptyKmTotal = r.EmptyKmTotal,
            EmptyKmSaved = r.EmptyKmSaved,
            PeriodStart = r.PeriodStart,
            PeriodEnd = r.PeriodEnd
        }).ToList();

        return new GetRouteUtilizationResponse
        {
            Routes = dtos
        };
    }
}
