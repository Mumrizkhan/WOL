namespace WOL.Shared.Messages.Commands;

public record SendNotificationCommand
{
    public Guid UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public Dictionary<string, string>? Data { get; init; }
    public string[] Channels { get; init; } = Array.Empty<string>();
    public string Type { get; init; } = string.Empty;
}
