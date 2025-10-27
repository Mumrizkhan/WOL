using WOL.Shared.Common.Application;

namespace WOL.Reporting.Domain.Repositories;

public interface IReportRepository : IRepository<Entities.Report>
{
    Task<IEnumerable<Entities.Report>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.Report>> GetPendingReportsAsync(CancellationToken cancellationToken = default);
}
