using WOL.Shared.Common.Application;

namespace WOL.Notification.Domain.Repositories;

public interface INotificationRepository : IRepository<Entities.Notification>
{
    Task<IEnumerable<Entities.Notification>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.Notification>> GetUnreadByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
