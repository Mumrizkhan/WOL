using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WOL.Backload.Application.Queries;

namespace WOL.Backload.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator,FleetManager")]
public class RouteHeatmapController : ControllerBase
{
    private readonly IMediator _mediator;

    public RouteHeatmapController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetRouteHeatmap([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var query = new GetRouteHeatmapQuery
        {
            StartDate = startDate ?? DateTime.UtcNow.AddMonths(-1),
            EndDate = endDate ?? DateTime.UtcNow
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("utilization")]
    public async Task<IActionResult> GetRouteUtilization([FromQuery] string? originCity, [FromQuery] string? destinationCity)
    {
        var query = new GetRouteUtilizationQuery
        {
            OriginCity = originCity,
            DestinationCity = destinationCity
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("imbalanced")]
    public async Task<IActionResult> GetImbalancedRoutes([FromQuery] decimal minImbalancePercentage = 30)
    {
        var query = new GetImbalancedRoutesQuery
        {
            MinImbalancePercentage = minImbalancePercentage
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
