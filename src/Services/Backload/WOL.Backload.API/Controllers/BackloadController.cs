using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WOL.Backload.Application.Commands;

namespace WOL.Backload.API.Controllers;

[ApiController]
[Route("api/backload")]
[Authorize]
public class BackloadController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BackloadController> _logger;

    public BackloadController(IMediator mediator, ILogger<BackloadController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("opportunities")]
    public async Task<ActionResult<CreateBackloadOpportunityResponse>> CreateOpportunity([FromBody] CreateBackloadOpportunityCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating backload opportunity");
            return BadRequest(new { message = ex.Message });
        }
    }
}
