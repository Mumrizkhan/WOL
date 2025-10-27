using WOL.Shared.Common.Application;

namespace WOL.Document.Domain.Repositories;

public interface IDocumentRepository : IRepository<Entities.Document>
{
    Task<IEnumerable<Entities.Document>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Entities.Document>> GetExpiringDocumentsAsync(int daysThreshold, CancellationToken cancellationToken = default);
}
