using MediatR;
using WOL.Identity.Domain.Entities;
using WOL.Identity.Domain.Repositories;

namespace WOL.Identity.Application.Queries;

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, IEnumerable<AuditLog>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetAuditLogsQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<IEnumerable<AuditLog>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId.HasValue)
        {
            return await _auditLogRepository.GetByUserIdAsync(
                request.UserId.Value,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
        }

        if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            return await _auditLogRepository.GetByDateRangeAsync(
                request.StartDate.Value,
                request.EndDate.Value,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
        }

        return Enumerable.Empty<AuditLog>();
    }
}
