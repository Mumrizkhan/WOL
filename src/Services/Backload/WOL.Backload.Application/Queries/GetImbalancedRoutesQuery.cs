using MediatR;
using WOL.Backload.Domain.Repositories;

namespace WOL.Backload.Application.Queries;

public record GetImbalancedRoutesQuery : IRequest<GetImbalancedRoutesResponse>
{
    public decimal MinImbalancePercentage { get; init; } = 30;
}

public record GetImbalancedRoutesResponse
{
    public List<ImbalancedRouteDto> Routes { get; init; } = new();
}

public record ImbalancedRouteDto
{
    public string OriginCity { get; init; } = string.Empty;
    public string DestinationCity { get; init; } = string.Empty;
    public int OutboundBookings { get; init; }
    public int ReturnBookings { get; init; }
    public decimal ImbalancePercentage { get; init; }
    public string Recommendation { get; init; } = string.Empty;
}

public class GetImbalancedRoutesQueryHandler : IRequestHandler<GetImbalancedRoutesQuery, GetImbalancedRoutesResponse>
{
    private readonly IRouteUtilizationRepository _routeUtilizationRepository;

    public GetImbalancedRoutesQueryHandler(IRouteUtilizationRepository routeUtilizationRepository)
    {
        _routeUtilizationRepository = routeUtilizationRepository;
    }

    public async Task<GetImbalancedRoutesResponse> Handle(GetImbalancedRoutesQuery request, CancellationToken cancellationToken)
    {
        var allRoutes = await _routeUtilizationRepository.GetAllAsync();

        var imbalancedRoutes = allRoutes
            .Where(r => r.OutboundBookings > 0)
            .Select(route =>
            {
                var imbalance = Math.Abs(route.OutboundBookings - route.ReturnBookings) / (decimal)route.OutboundBookings * 100;
                var recommendation = route.OutboundBookings > route.ReturnBookings
                    ? $"Promote backload opportunities from {route.DestinationCity} to {route.OriginCity}"
                    : $"Promote backload opportunities from {route.OriginCity} to {route.DestinationCity}";

                return new ImbalancedRouteDto
                {
                    OriginCity = route.OriginCity,
                    DestinationCity = route.DestinationCity,
                    OutboundBookings = route.OutboundBookings,
                    ReturnBookings = route.ReturnBookings,
                    ImbalancePercentage = imbalance,
                    Recommendation = recommendation
                };
            })
            .Where(r => r.ImbalancePercentage >= request.MinImbalancePercentage)
            .OrderByDescending(r => r.ImbalancePercentage)
            .ToList();

        return new GetImbalancedRoutesResponse
        {
            Routes = imbalancedRoutes
        };
    }
}
