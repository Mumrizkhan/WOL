using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WOL.Identity.Application.Commands;
using WOL.Identity.Application.Queries;

namespace WOL.Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("assign")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
    {
        var command = new AssignRoleCommand(request.UserId, request.RoleName);
        var result = await _mediator.Send(command);

        if (!result)
            return BadRequest(new { message = "Failed to assign role" });

        return Ok(new { message = "Role assigned successfully" });
    }

    [HttpPost("remove")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> RemoveRole([FromBody] RemoveRoleRequest request)
    {
        var command = new RemoveRoleCommand(request.UserId, request.RoleName);
        var result = await _mediator.Send(command);

        if (!result)
            return BadRequest(new { message = "Failed to remove role" });

        return Ok(new { message = "Role removed successfully" });
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserRoles(Guid userId)
    {
        var query = new GetUserRolesQuery(userId);
        var roles = await _mediator.Send(query);

        return Ok(roles);
    }
}

public record AssignRoleRequest(Guid UserId, string RoleName);
public record RemoveRoleRequest(Guid UserId, string RoleName);
