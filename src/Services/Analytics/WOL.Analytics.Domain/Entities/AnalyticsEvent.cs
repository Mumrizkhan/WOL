using WOL.Shared.Common.Domain;

namespace WOL.Analytics.Domain.Entities;

public class AnalyticsEvent : BaseEntity
{
    public string EventType { get; private set; } = string.Empty;
    public string EventCategory { get; private set; } = string.Empty;
    public Guid? UserId { get; private set; }
    public string? EntityId { get; private set; }
    public string? EntityType { get; private set; }
    public string Metadata { get; private set; } = "{}";
    public DateTime EventTimestamp { get; private set; }

    private AnalyticsEvent() { }

    public static AnalyticsEvent Create(string eventType, string eventCategory, Guid? userId, string? entityId, string? entityType, string metadata)
    {
        return new AnalyticsEvent
        {
            EventType = eventType,
            EventCategory = eventCategory,
            UserId = userId,
            EntityId = entityId,
            EntityType = entityType,
            Metadata = metadata,
            EventTimestamp = DateTime.UtcNow
        };
    }
}
