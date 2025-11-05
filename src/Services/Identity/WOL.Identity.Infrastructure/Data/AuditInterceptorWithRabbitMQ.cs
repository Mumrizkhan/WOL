using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using MassTransit;
using WOL.Identity.Domain.Entities;
using WOL.Identity.Domain.Enums;
using WOL.Shared.Messages.Events;

namespace WOL.Identity.Infrastructure.Data;

public class AuditInterceptorWithRabbitMQ : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuditInterceptorWithRabbitMQ(
        IHttpContextAccessor httpContextAccessor,
        IPublishEndpoint publishEndpoint)
    {
        _httpContextAccessor = httpContextAccessor;
        _publishEndpoint = publishEndpoint;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateAuditFieldsAndPublishEvents(eventData.Context).GetAwaiter().GetResult();
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        await UpdateAuditFieldsAndPublishEvents(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task UpdateAuditFieldsAndPublishEvents(DbContext? context)
    {
        if (context == null) return;

        var httpContext = _httpContextAccessor.HttpContext;
        var userId = httpContext?.User?.FindFirst("sub")?.Value;
        var username = httpContext?.User?.Identity?.Name;
        var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString();
        var userAgent = httpContext?.Request?.Headers["User-Agent"].ToString();

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.Entity is ApplicationUser && 
                       (e.State == EntityState.Added || 
                        e.State == EntityState.Modified || 
                        e.State == EntityState.Deleted))
            .ToList();

        foreach (var entry in entries)
        {
            var user = (ApplicationUser)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                user.CreatedAt = DateTime.UtcNow;
                user.CreatedBy = userId;
                
                await PublishAuditEvent(AuditAction.Create, user, null, entry, ipAddress, userAgent, username);
            }
            else if (entry.State == EntityState.Modified)
            {
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = userId;
                
                var oldValues = GetOldValues(entry);
                var newValues = GetNewValues(entry);
                
                await PublishAuditEvent(AuditAction.Update, user, oldValues, entry, ipAddress, userAgent, username, newValues);
            }
            else if (entry.State == EntityState.Deleted)
            {
                var oldValues = GetOldValues(entry);
                await PublishAuditEvent(AuditAction.Delete, user, oldValues, entry, ipAddress, userAgent, username);
            }
        }
    }

    private async Task PublishAuditEvent(
        AuditAction action,
        ApplicationUser user,
        Dictionary<string, object?>? oldValues,
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry,
        string? ipAddress,
        string? userAgent,
        string? username,
        Dictionary<string, object?>? newValues = null)
    {
        var auditEvent = new AuditLogCreatedEvent
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Username = username ?? user.UserName,
            Action = action.ToString(),
            EntityName = entry.Entity.GetType().Name,
            EntityId = user.Id.ToString(),
            OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Timestamp = DateTime.UtcNow
        };

        await _publishEndpoint.Publish(auditEvent);
    }

    private Dictionary<string, object?> GetOldValues(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
    {
        var result = new Dictionary<string, object?>();
        
        foreach (var property in entry.OriginalValues.Properties)
        {
            var originalValue = entry.OriginalValues[property];
            result[property.Name] = originalValue;
        }
        
        return result;
    }

    private Dictionary<string, object?> GetNewValues(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
    {
        var result = new Dictionary<string, object?>();
        
        foreach (var property in entry.CurrentValues.Properties)
        {
            var currentValue = entry.CurrentValues[property];
            result[property.Name] = currentValue;
        }
        
        return result;
    }
}
