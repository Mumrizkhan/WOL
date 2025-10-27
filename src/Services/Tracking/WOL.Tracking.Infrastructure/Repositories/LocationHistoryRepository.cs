using Microsoft.EntityFrameworkCore;
using WOL.Tracking.Domain.Entities;
using WOL.Tracking.Domain.Repositories;
using WOL.Tracking.Infrastructure.Data;

namespace WOL.Tracking.Infrastructure.Repositories;

public class LocationHistoryRepository : ILocationHistoryRepository
{
    private readonly TrackingDbContext _context;

    public LocationHistoryRepository(TrackingDbContext context)
    {
        _context = context;
    }

    public async Task<LocationHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LocationHistories.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<LocationHistory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LocationHistories.ToListAsync(cancellationToken);
    }

    public async Task<LocationHistory> AddAsync(LocationHistory entity, CancellationToken cancellationToken = default)
    {
        await _context.LocationHistories.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(LocationHistory entity, CancellationToken cancellationToken = default)
    {
        _context.LocationHistories.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(LocationHistory entity, CancellationToken cancellationToken = default)
    {
        _context.LocationHistories.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<LocationHistory>> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return await _context.LocationHistories
            .Where(l => l.BookingId == bookingId)
            .OrderBy(l => l.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<LocationHistory?> GetLatestLocationAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        return await _context.LocationHistories
            .Where(l => l.BookingId == bookingId)
            .OrderByDescending(l => l.Timestamp)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
