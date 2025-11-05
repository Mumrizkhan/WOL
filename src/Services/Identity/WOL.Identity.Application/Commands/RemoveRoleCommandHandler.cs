using MediatR;
using Microsoft.AspNetCore.Identity;
using MassTransit;
using WOL.Identity.Domain.Entities;
using WOL.Identity.Domain.Enums;
using WOL.Shared.Messages.Events;

namespace WOL.Identity.Application.Commands;

public class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPublishEndpoint _publishEndpoint;

    public RemoveRoleCommandHandler(
        UserManager<ApplicationUser> userManager,
        IPublishEndpoint publishEndpoint)
    {
        _userManager = userManager;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return false;

        var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);

        if (result.Succeeded)
        {
            await _publishEndpoint.Publish(new AuditLogCreatedEvent
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Username = user.UserName,
                Action = AuditAction.RoleRemoved.ToString(),
                EntityName = "UserRole",
                EntityId = request.UserId.ToString(),
                OldValues = $"Role: {request.RoleName}",
                Timestamp = DateTime.UtcNow
            }, cancellationToken);
        }

        return result.Succeeded;
    }
}
