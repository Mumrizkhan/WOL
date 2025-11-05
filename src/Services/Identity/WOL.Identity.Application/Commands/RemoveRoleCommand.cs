using MediatR;

namespace WOL.Identity.Application.Commands;

public record RemoveRoleCommand(Guid UserId, string RoleName) : IRequest<bool>;
