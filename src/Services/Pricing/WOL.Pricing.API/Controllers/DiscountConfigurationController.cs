using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WOL.Pricing.Application.Commands;
using WOL.Pricing.Application.Queries;

namespace WOL.Pricing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class DiscountConfigurationController : ControllerBase
{
    private readonly IMediator _mediator;

    public DiscountConfigurationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDiscounts()
    {
        var query = new GetAllDiscountConfigurationsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{discountType}")]
    public async Task<IActionResult> GetDiscountByType(string discountType)
    {
        var query = new GetDiscountConfigurationByTypeQuery { DiscountType = discountType };
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDiscount(Guid id, [FromBody] UpdateDiscountConfigurationCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/activate")]
    public async Task<IActionResult> ActivateDiscount(Guid id)
    {
        var command = new ActivateDiscountConfigurationCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivateDiscount(Guid id)
    {
        var command = new DeactivateDiscountConfigurationCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
