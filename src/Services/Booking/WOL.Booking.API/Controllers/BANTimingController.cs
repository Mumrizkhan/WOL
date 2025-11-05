using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WOL.Booking.Application.Commands;
using WOL.Booking.Application.Queries;

namespace WOL.Booking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class BANTimingController : ControllerBase
{
    private readonly IMediator _mediator;

    public BANTimingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBANTimings()
    {
        var query = new GetAllBANTimingsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{city}")]
    public async Task<IActionResult> GetBANTimingsByCity(string city)
    {
        var query = new GetBANTimingsByCityQuery { City = city };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBANTiming([FromBody] CreateBANTimingCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBANTiming(Guid id, [FromBody] UpdateBANTimingCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBANTiming(Guid id)
    {
        var command = new DeleteBANTimingCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/activate")]
    public async Task<IActionResult> ActivateBANTiming(Guid id)
    {
        var command = new ActivateBANTimingCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivateBANTiming(Guid id)
    {
        var command = new DeactivateBANTimingCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
