using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WOL.Pricing.Application.Commands;

namespace WOL.Pricing.API.Controllers;

[ApiController]
[Route("api/pricing")]
[Authorize]
public class PricingController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PricingController> _logger;

    public PricingController(IMediator mediator, ILogger<PricingController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("calculate")]
    public async Task<ActionResult<CalculatePriceResponse>> CalculatePrice([FromBody] CalculatePriceCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating price");
            return BadRequest(new { message = ex.Message });
        }
    }
}
