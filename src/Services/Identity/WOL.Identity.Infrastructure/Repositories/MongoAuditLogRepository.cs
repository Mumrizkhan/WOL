using MongoDB.Driver;
using WOL.Identity.Domain.Entities;
using WOL.Identity.Domain.Enums;
using WOL.Identity.Domain.Repositories;

namespace WOL.Identity.Infrastructure.Repositories;

public class MongoAuditLogRepository : IAuditLogRepository
{
    private readonly IMongoCollection<AuditLogDocument> _auditLogsCollection;

    public MongoAuditLogRepository(IMongoDatabase database)
    {
        _auditLogsCollection = database.GetCollection<AuditLogDocument>("audit_logs");
    }

    public Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Audit logs are now written by the AuditWorker service via RabbitMQ events");
    }

    public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var filter = Builders<AuditLogDocument>.Filter.Eq(d => d.UserId, userId.ToString());
        
        var documents = await _auditLogsCollection
            .Find(filter)
            .SortByDescending(d => d.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return documents.Select(MapToAuditLog);
    }

    public async Task<IEnumerable<AuditLog>> GetByActionAsync(AuditAction action, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var filter = Builders<AuditLogDocument>.Filter.Eq(d => d.Action, action.ToString());
        
        var documents = await _auditLogsCollection
            .Find(filter)
            .SortByDescending(d => d.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return documents.Select(MapToAuditLog);
    }

    public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var filter = Builders<AuditLogDocument>.Filter.And(
            Builders<AuditLogDocument>.Filter.Gte(d => d.Timestamp, startDate),
            Builders<AuditLogDocument>.Filter.Lte(d => d.Timestamp, endDate)
        );
        
        var documents = await _auditLogsCollection
            .Find(filter)
            .SortByDescending(d => d.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return documents.Select(MapToAuditLog);
    }

    private AuditLog MapToAuditLog(AuditLogDocument document)
    {
        Guid? userId = null;
        if (!string.IsNullOrEmpty(document.UserId) && Guid.TryParse(document.UserId, out var parsedUserId))
        {
            userId = parsedUserId;
        }

        AuditAction action = AuditAction.Create;
        if (Enum.TryParse<AuditAction>(document.Action, out var parsedAction))
        {
            action = parsedAction;
        }

        return new AuditLog
        {
            Id = Guid.Parse(document.Id),
            UserId = userId,
            Username = document.Username,
            Action = action,
            EntityName = document.EntityName,
            EntityId = document.EntityId,
            OldValues = document.OldValues,
            NewValues = document.NewValues,
            IpAddress = document.IpAddress,
            UserAgent = document.UserAgent,
            Timestamp = document.Timestamp
        };
    }
}

public class AuditLogDocument
{
    public string Id { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? Username { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
}
