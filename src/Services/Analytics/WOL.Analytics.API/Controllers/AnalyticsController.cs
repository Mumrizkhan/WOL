using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WOL.Analytics.Application.Commands;

namespace WOL.Analytics.API.Controllers;

[ApiController]
[Route("api/analytics")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(IMediator mediator, ILogger<AnalyticsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("track")]
    public async Task<ActionResult<TrackEventResponse>> TrackEvent([FromBody] TrackEventCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking analytics event");
            return BadRequest(new { message = ex.Message });
        }
    }
}
