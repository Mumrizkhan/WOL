using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using WOL.Shared.Messages.Events;

namespace WOL.AuditWorker;

public class AuditLogCreatedConsumer : IConsumer<AuditLogCreatedEvent>
{
    private readonly IMongoCollection<AuditLogDocument> _auditLogsCollection;
    private readonly ILogger<AuditLogCreatedConsumer> _logger;

    public AuditLogCreatedConsumer(
        IMongoDatabase database,
        ILogger<AuditLogCreatedConsumer> logger)
    {
        _auditLogsCollection = database.GetCollection<AuditLogDocument>("audit_logs");
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<AuditLogCreatedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Processing AuditLogCreatedEvent: {Action} on {EntityName} by User {UserId}",
            message.Action,
            message.EntityName,
            message.UserId);

        try
        {
            var auditLog = new AuditLogDocument
            {
                Id = message.Id.ToString(),
                UserId = message.UserId?.ToString(),
                Username = message.Username,
                Action = message.Action,
                EntityName = message.EntityName,
                EntityId = message.EntityId,
                OldValues = message.OldValues,
                NewValues = message.NewValues,
                IpAddress = message.IpAddress,
                UserAgent = message.UserAgent,
                Timestamp = message.Timestamp
            };

            await _auditLogsCollection.InsertOneAsync(auditLog);

            _logger.LogInformation(
                "Successfully stored audit log {AuditLogId} for action {Action}",
                message.Id,
                message.Action);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to store audit log {AuditLogId} for action {Action}",
                message.Id,
                message.Action);
            throw;
        }
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
