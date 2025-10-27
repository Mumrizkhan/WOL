using WOL.Shared.Common.Domain;
using WOL.Identity.Domain.Enums;

namespace WOL.Identity.Domain.Events;

public class UserCreatedEvent : IDomainEvent
{
    public Guid UserId { get; init; }
    public UserType UserType { get; init; }
    public string MobileNumber { get; init; } = string.Empty;
    public string? Email { get; init; }
    public DateTime OccurredOn { get; init; }
}
