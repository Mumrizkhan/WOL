using MediatR;

namespace WOL.Notification.Application.Commands;

public record SendNotificationCommand : IRequest<SendNotificationResponse>
{
    public Guid UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public Dictionary<string, string>? Data { get; init; }
}

public record SendNotificationResponse
{
    public Guid NotificationId { get; init; }
    public string Message { get; init; } = string.Empty;
}
