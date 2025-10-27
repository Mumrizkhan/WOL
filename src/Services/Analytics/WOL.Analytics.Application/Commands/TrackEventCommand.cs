using MediatR;

namespace WOL.Analytics.Application.Commands;

public record TrackEventCommand : IRequest<TrackEventResponse>
{
    public string EventType { get; init; } = string.Empty;
    public string EventCategory { get; init; } = string.Empty;
    public Guid? UserId { get; init; }
    public string? EntityId { get; init; }
    public string? EntityType { get; init; }
    public string Metadata { get; init; } = "{}";
}

public record TrackEventResponse
{
    public Guid EventId { get; init; }
    public string Message { get; init; } = string.Empty;
}
