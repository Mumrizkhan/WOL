using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WOL.Vehicle.Application.Commands;

namespace WOL.Vehicle.API.Controllers;

[ApiController]
[Route("api/vehicles")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<VehiclesController> _logger;

    public VehiclesController(IMediator mediator, ILogger<VehiclesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<RegisterVehicleResponse>> RegisterVehicle([FromBody] RegisterVehicleCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(RegisterVehicle), new { id = response.VehicleId }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering vehicle");
            return BadRequest(new { message = ex.Message });
        }
    }
}
