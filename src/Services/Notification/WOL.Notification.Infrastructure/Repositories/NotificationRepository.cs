using Microsoft.EntityFrameworkCore;
using WOL.Notification.Domain.Repositories;
using WOL.Notification.Infrastructure.Data;

namespace WOL.Notification.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _context;

    public NotificationRepository(NotificationDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Notification>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Notifications.ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Notification> AddAsync(Domain.Entities.Notification entity, CancellationToken cancellationToken = default)
    {
        await _context.Notifications.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(Domain.Entities.Notification entity, CancellationToken cancellationToken = default)
    {
        _context.Notifications.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Domain.Entities.Notification entity, CancellationToken cancellationToken = default)
    {
        _context.Notifications.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Domain.Entities.Notification>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Notification>> GetUnreadByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
