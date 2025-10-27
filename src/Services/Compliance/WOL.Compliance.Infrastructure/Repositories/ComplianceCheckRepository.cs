using Microsoft.EntityFrameworkCore;
using WOL.Compliance.Domain.Entities;
using WOL.Compliance.Domain.Repositories;
using WOL.Compliance.Infrastructure.Data;

namespace WOL.Compliance.Infrastructure.Repositories;

public class ComplianceCheckRepository : IComplianceCheckRepository
{
    private readonly ComplianceDbContext _context;

    public ComplianceCheckRepository(ComplianceDbContext context)
    {
        _context = context;
    }

    public async Task<ComplianceCheck?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ComplianceChecks.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<ComplianceCheck>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ComplianceChecks.ToListAsync(cancellationToken);
    }

    public async Task<ComplianceCheck> AddAsync(ComplianceCheck entity, CancellationToken cancellationToken = default)
    {
        await _context.ComplianceChecks.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(ComplianceCheck entity, CancellationToken cancellationToken = default)
    {
        _context.ComplianceChecks.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(ComplianceCheck entity, CancellationToken cancellationToken = default)
    {
        _context.ComplianceChecks.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<ComplianceCheck>> GetByEntityIdAsync(Guid entityId, CancellationToken cancellationToken = default)
    {
        return await _context.ComplianceChecks
            .Where(c => c.EntityId == entityId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ComplianceCheck>> GetPendingChecksAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ComplianceChecks
            .Where(c => c.Status == "Pending")
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
