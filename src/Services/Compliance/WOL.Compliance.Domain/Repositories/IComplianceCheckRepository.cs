using WOL.Shared.Common.Application;

namespace WOL.Compliance.Domain.Repositories;

public interface IComplianceCheckRepository : IRepository<Entities.ComplianceCheck>
{
    Task<IEnumerable<Entities.ComplianceCheck>> GetByEntityIdAsync(Guid entityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.ComplianceCheck>> GetPendingChecksAsync(CancellationToken cancellationToken = default);
}
