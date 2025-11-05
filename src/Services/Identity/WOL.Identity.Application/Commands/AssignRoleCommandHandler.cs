using MediatR;
using Microsoft.AspNetCore.Identity;
using WOL.Identity.Domain.Entities;
using WOL.Identity.Domain.Enums;
using WOL.Identity.Domain.Repositories;

namespace WOL.Identity.Application.Commands;

public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IAuditLogRepository _auditLogRepository;

    public AssignRoleCommandHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IAuditLogRepository auditLogRepository)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<bool> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return false;

        var roleExists = await _roleManager.RoleExistsAsync(request.RoleName);
        if (!roleExists)
            return false;

        var result = await _userManager.AddToRoleAsync(user, request.RoleName);

        if (result.Succeeded)
        {
            await _auditLogRepository.AddAsync(new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Username = user.UserName,
                Action = AuditAction.RoleAssigned,
                EntityName = "UserRole",
                EntityId = request.UserId.ToString(),
                NewValues = $"Role: {request.RoleName}",
                Timestamp = DateTime.UtcNow
            }, cancellationToken);
        }

        return result.Succeeded;
    }
}
