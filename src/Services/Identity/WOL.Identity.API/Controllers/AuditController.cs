using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WOL.Identity.Application.Queries;

namespace WOL.Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class AuditController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuditController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserAuditLogs(
        Guid userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = new GetAuditLogsQuery(
            UserId: userId,
            PageNumber: pageNumber,
            PageSize: pageSize);

        var logs = await _mediator.Send(query);

        return Ok(logs);
    }

    [HttpGet("date-range")]
    public async Task<IActionResult> GetAuditLogsByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = new GetAuditLogsQuery(
            StartDate: startDate,
            EndDate: endDate,
            PageNumber: pageNumber,
            PageSize: pageSize);

        var logs = await _mediator.Send(query);

        return Ok(logs);
    }
}
