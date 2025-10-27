using Microsoft.EntityFrameworkCore;
using WOL.Reporting.Domain.Entities;
using WOL.Reporting.Domain.Repositories;
using WOL.Reporting.Infrastructure.Data;

namespace WOL.Reporting.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ReportingDbContext _context;

    public ReportRepository(ReportingDbContext context)
    {
        _context = context;
    }

    public async Task<Report?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Reports.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Report>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Reports.ToListAsync(cancellationToken);
    }

    public async Task<Report> AddAsync(Report entity, CancellationToken cancellationToken = default)
    {
        await _context.Reports.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(Report entity, CancellationToken cancellationToken = default)
    {
        _context.Reports.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Report entity, CancellationToken cancellationToken = default)
    {
        _context.Reports.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Report>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .Where(r => r.RequestedBy == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Report>> GetPendingReportsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .Where(r => r.Status == "Pending")
            .OrderBy(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
