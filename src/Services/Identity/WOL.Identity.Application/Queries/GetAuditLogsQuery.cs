using MediatR;
using WOL.Identity.Domain.Entities;

namespace WOL.Identity.Application.Queries;

public record GetAuditLogsQuery(
    Guid? UserId = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int PageNumber = 1,
    int PageSize = 50) : IRequest<IEnumerable<AuditLog>>;
