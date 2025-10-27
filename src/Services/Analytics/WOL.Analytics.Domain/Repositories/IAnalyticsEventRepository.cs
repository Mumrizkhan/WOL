using WOL.Shared.Common.Application;

namespace WOL.Analytics.Domain.Repositories;

public interface IAnalyticsEventRepository : IRepository<Entities.AnalyticsEvent>
{
    Task<IEnumerable<Entities.AnalyticsEvent>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.AnalyticsEvent>> GetByEventTypeAsync(string eventType, DateTime from, DateTime to, CancellationToken cancellationToken = default);
}
