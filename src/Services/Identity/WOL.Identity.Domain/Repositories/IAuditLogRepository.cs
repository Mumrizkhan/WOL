using WOL.Identity.Domain.Entities;
using WOL.Identity.Domain.Enums;

namespace WOL.Identity.Domain.Repositories;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByActionAsync(AuditAction action, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);
}
