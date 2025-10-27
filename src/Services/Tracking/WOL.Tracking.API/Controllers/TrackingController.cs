using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WOL.Tracking.Application.Commands;

namespace WOL.Tracking.API.Controllers;

[ApiController]
[Route("api/tracking")]
[Authorize]
public class TrackingController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TrackingController> _logger;

    public TrackingController(IMediator mediator, ILogger<TrackingController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("location")]
    public async Task<ActionResult<RecordLocationResponse>> RecordLocation([FromBody] RecordLocationCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording location");
            return BadRequest(new { message = ex.Message });
        }
    }
}
