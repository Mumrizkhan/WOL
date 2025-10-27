using WOL.Shared.Common.Domain;

namespace WOL.Notification.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public Dictionary<string, string> Data { get; private set; } = new();

    private Notification() { }

    public static Notification Create(Guid userId, string title, string body, string type, Dictionary<string, string>? data = null)
    {
        return new Notification
        {
            UserId = userId,
            Title = title,
            Body = body,
            Type = type,
            IsRead = false,
            Data = data ?? new Dictionary<string, string>()
        };
    }

    public void MarkAsRead()
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
        SetUpdatedAt();
    }
}
