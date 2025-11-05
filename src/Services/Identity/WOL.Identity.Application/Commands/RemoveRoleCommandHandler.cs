using MediatR;
using Microsoft.AspNetCore.Identity;
using WOL.Identity.Domain.Entities;
using WOL.Identity.Domain.Enums;
using WOL.Identity.Domain.Repositories;

namespace WOL.Identity.Application.Commands;

public class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuditLogRepository _auditLogRepository;

    public RemoveRoleCommandHandler(
        UserManager<ApplicationUser> userManager,
        IAuditLogRepository auditLogRepository)
    {
        _userManager = userManager;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<bool> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return false;

        var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);

        if (result.Succeeded)
        {
            await _auditLogRepository.AddAsync(new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Username = user.UserName,
                Action = AuditAction.RoleRemoved,
                EntityName = "UserRole",
                EntityId = request.UserId.ToString(),
                OldValues = $"Role: {request.RoleName}",
                Timestamp = DateTime.UtcNow
            }, cancellationToken);
        }

        return result.Succeeded;
    }
}
