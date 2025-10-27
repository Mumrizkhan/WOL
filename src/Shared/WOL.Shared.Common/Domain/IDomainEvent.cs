using MediatR;

namespace WOL.Shared.Common.Domain;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
