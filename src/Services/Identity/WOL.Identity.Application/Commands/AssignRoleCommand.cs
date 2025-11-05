using MediatR;

namespace WOL.Identity.Application.Commands;

public record AssignRoleCommand(Guid UserId, string RoleName) : IRequest<bool>;
