using Microsoft.EntityFrameworkCore;
using WOL.Backload.Domain.Entities;
using WOL.Backload.Domain.Repositories;
using WOL.Backload.Infrastructure.Data;

namespace WOL.Backload.Infrastructure.Repositories;

public class BackloadOpportunityRepository : IBackloadOpportunityRepository
{
    private readonly BackloadDbContext _context;

    public BackloadOpportunityRepository(BackloadDbContext context)
    {
        _context = context;
    }

    public async Task<BackloadOpportunity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.BackloadOpportunities.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<BackloadOpportunity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BackloadOpportunities.ToListAsync(cancellationToken);
    }

    public async Task<BackloadOpportunity> AddAsync(BackloadOpportunity entity, CancellationToken cancellationToken = default)
    {
        await _context.BackloadOpportunities.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(BackloadOpportunity entity, CancellationToken cancellationToken = default)
    {
        _context.BackloadOpportunities.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(BackloadOpportunity entity, CancellationToken cancellationToken = default)
    {
        _context.BackloadOpportunities.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<BackloadOpportunity>> GetAvailableOpportunitiesAsync(string fromCity, string toCity, CancellationToken cancellationToken = default)
    {
        return await _context.BackloadOpportunities
            .Where(b => b.FromCity == fromCity && b.ToCity == toCity && b.Status == "Available")
            .OrderBy(b => b.AvailableFrom)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BackloadOpportunity>> GetByDriverIdAsync(Guid driverId, CancellationToken cancellationToken = default)
    {
        return await _context.BackloadOpportunities
            .Where(b => b.DriverId == driverId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
