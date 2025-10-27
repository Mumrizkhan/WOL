using Microsoft.EntityFrameworkCore;
using WOL.Document.Domain.Repositories;
using WOL.Document.Infrastructure.Data;

namespace WOL.Document.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly DocumentDbContext _context;

    public DocumentRepository(DocumentDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Document?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Documents.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Document>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Documents.ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Document> AddAsync(Domain.Entities.Document entity, CancellationToken cancellationToken = default)
    {
        await _context.Documents.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(Domain.Entities.Document entity, CancellationToken cancellationToken = default)
    {
        _context.Documents.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Domain.Entities.Document entity, CancellationToken cancellationToken = default)
    {
        _context.Documents.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Domain.Entities.Document>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Documents
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Document>> GetExpiringDocumentsAsync(int daysThreshold, CancellationToken cancellationToken = default)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);
        return await _context.Documents
            .Where(d => d.ExpiryDate <= thresholdDate && d.ExpiryDate >= DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }
}
