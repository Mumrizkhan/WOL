using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WOL.Pricing.Application.Commands;
using WOL.Pricing.Application.Queries;

namespace WOL.Pricing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class FeeConfigurationController : ControllerBase
{
    private readonly IMediator _mediator;

    public FeeConfigurationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllFees()
    {
        var query = new GetAllFeeConfigurationsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{feeType}")]
    public async Task<IActionResult> GetFeeByType(string feeType)
    {
        var query = new GetFeeConfigurationByTypeQuery { FeeType = feeType };
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFee(Guid id, [FromBody] UpdateFeeConfigurationCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/activate")]
    public async Task<IActionResult> ActivateFee(Guid id)
    {
        var command = new ActivateFeeConfigurationCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivateFee(Guid id)
    {
        var command = new DeactivateFeeConfigurationCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
