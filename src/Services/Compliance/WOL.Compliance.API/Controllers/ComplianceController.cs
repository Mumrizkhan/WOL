using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WOL.Compliance.Application.Commands;

namespace WOL.Compliance.API.Controllers;

[ApiController]
[Route("api/compliance")]
[Authorize]
public class ComplianceController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ComplianceController> _logger;

    public ComplianceController(IMediator mediator, ILogger<ComplianceController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("checks")]
    public async Task<ActionResult<CreateComplianceCheckResponse>> CreateCheck([FromBody] CreateComplianceCheckCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating compliance check");
            return BadRequest(new { message = ex.Message });
        }
    }
}
