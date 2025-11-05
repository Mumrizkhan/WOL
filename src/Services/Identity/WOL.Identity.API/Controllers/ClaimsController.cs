using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WOL.Identity.Application.Commands;
using WOL.Identity.Application.Queries;

namespace WOL.Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClaimsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClaimsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("add")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> AddClaim([FromBody] AddClaimRequest request)
    {
        var command = new AddClaimCommand(request.UserId, request.ClaimType, request.ClaimValue);
        var result = await _mediator.Send(command);

        if (!result)
            return BadRequest(new { message = "Failed to add claim" });

        return Ok(new { message = "Claim added successfully" });
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserClaims(Guid userId)
    {
        var query = new GetUserClaimsQuery(userId);
        var claims = await _mediator.Send(query);

        return Ok(claims.Select(c => new { c.Type, c.Value }));
    }
}

public record AddClaimRequest(Guid UserId, string ClaimType, string ClaimValue);
