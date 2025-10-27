using Microsoft.EntityFrameworkCore;
using WOL.Analytics.Domain.Entities;
using WOL.Analytics.Domain.Repositories;
using WOL.Analytics.Infrastructure.Data;

namespace WOL.Analytics.Infrastructure.Repositories;

public class AnalyticsEventRepository : IAnalyticsEventRepository
{
    private readonly AnalyticsDbContext _context;

    public AnalyticsEventRepository(AnalyticsDbContext context)
    {
        _context = context;
    }

    public async Task<AnalyticsEvent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AnalyticsEvents.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<AnalyticsEvent>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AnalyticsEvents.ToListAsync(cancellationToken);
    }

    public async Task<AnalyticsEvent> AddAsync(AnalyticsEvent entity, CancellationToken cancellationToken = default)
    {
        await _context.AnalyticsEvents.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(AnalyticsEvent entity, CancellationToken cancellationToken = default)
    {
        _context.AnalyticsEvents.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(AnalyticsEvent entity, CancellationToken cancellationToken = default)
    {
        _context.AnalyticsEvents.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<AnalyticsEvent>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.AnalyticsEvents
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.EventTimestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AnalyticsEvent>> GetByEventTypeAsync(string eventType, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await _context.AnalyticsEvents
            .Where(e => e.EventType == eventType && e.EventTimestamp >= from && e.EventTimestamp <= to)
            .OrderBy(e => e.EventTimestamp)
            .ToListAsync(cancellationToken);
    }
}
