using MediatR;
using WOL.Backload.Domain.Repositories;

namespace WOL.Backload.Application.Queries;

public record GetRouteHeatmapQuery : IRequest<GetRouteHeatmapResponse>
{
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}

public record GetRouteHeatmapResponse
{
    public List<RouteHeatmapData> Routes { get; init; } = new();
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}

public record RouteHeatmapData
{
    public string OriginCity { get; init; } = string.Empty;
    public string DestinationCity { get; init; } = string.Empty;
    public int OutboundBookings { get; init; }
    public int ReturnBookings { get; init; }
    public decimal UtilizationPercentage { get; init; }
    public decimal ImbalancePercentage { get; init; }
    public decimal EmptyKmTotal { get; init; }
    public decimal EmptyKmSaved { get; init; }
    public string FlowDirection { get; init; } = string.Empty;
}

public class GetRouteHeatmapQueryHandler : IRequestHandler<GetRouteHeatmapQuery, GetRouteHeatmapResponse>
{
    private readonly IRouteUtilizationRepository _routeUtilizationRepository;

    public GetRouteHeatmapQueryHandler(IRouteUtilizationRepository routeUtilizationRepository)
    {
        _routeUtilizationRepository = routeUtilizationRepository;
    }

    public async Task<GetRouteHeatmapResponse> Handle(GetRouteHeatmapQuery request, CancellationToken cancellationToken)
    {
        var allRoutes = await _routeUtilizationRepository.GetAllAsync();
        
        var filteredRoutes = allRoutes
            .Where(r => r.PeriodStart >= request.StartDate && r.PeriodEnd <= request.EndDate)
            .ToList();

        var heatmapData = filteredRoutes.Select(route =>
        {
            var imbalance = route.OutboundBookings > 0 
                ? Math.Abs(route.OutboundBookings - route.ReturnBookings) / (decimal)route.OutboundBookings * 100 
                : 0;

            var flowDirection = route.OutboundBookings > route.ReturnBookings 
                ? "OUTBOUND_HEAVY" 
                : route.ReturnBookings > route.OutboundBookings 
                    ? "RETURN_HEAVY" 
                    : "BALANCED";

            return new RouteHeatmapData
            {
                OriginCity = route.OriginCity,
                DestinationCity = route.DestinationCity,
                OutboundBookings = route.OutboundBookings,
                ReturnBookings = route.ReturnBookings,
                UtilizationPercentage = route.UtilizationPercentage,
                ImbalancePercentage = imbalance,
                EmptyKmTotal = route.EmptyKmTotal,
                EmptyKmSaved = route.EmptyKmSaved,
                FlowDirection = flowDirection
            };
        }).ToList();

        return new GetRouteHeatmapResponse
        {
            Routes = heatmapData,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };
    }
}
