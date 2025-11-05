using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using WOL.Identity.Domain.Entities;
using WOL.Identity.Domain.Enums;

namespace WOL.Identity.Infrastructure.Data;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditFields(DbContext? context)
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
                
                CreateAuditLog(context, AuditAction.Create, user, null, entry, ipAddress, userAgent, username);
            }
            else if (entry.State == EntityState.Modified)
            {
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = userId;
                
                var oldValues = GetOldValues(entry);
                var newValues = GetNewValues(entry);
                
                CreateAuditLog(context, AuditAction.Update, user, oldValues, entry, ipAddress, userAgent, username, newValues);
            }
            else if (entry.State == EntityState.Deleted)
            {
                var oldValues = GetOldValues(entry);
                CreateAuditLog(context, AuditAction.Delete, user, oldValues, entry, ipAddress, userAgent, username);
            }
        }
    }

    private void CreateAuditLog(
        DbContext context,
        AuditAction action,
        ApplicationUser user,
        Dictionary<string, object?>? oldValues,
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry,
        string? ipAddress,
        string? userAgent,
        string? username,
        Dictionary<string, object?>? newValues = null)
    {
        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Username = username ?? user.UserName,
            Action = action,
            EntityName = entry.Entity.GetType().Name,
            EntityId = user.Id.ToString(),
            OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Timestamp = DateTime.UtcNow
        };

        context.Set<AuditLog>().Add(auditLog);
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
