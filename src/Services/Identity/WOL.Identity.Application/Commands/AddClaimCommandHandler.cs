using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WOL.Identity.Domain.Entities;
using WOL.Identity.Domain.Enums;
using WOL.Identity.Domain.Repositories;

namespace WOL.Identity.Application.Commands;

public class AddClaimCommandHandler : IRequestHandler<AddClaimCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuditLogRepository _auditLogRepository;

    public AddClaimCommandHandler(
        UserManager<ApplicationUser> userManager,
        IAuditLogRepository auditLogRepository)
    {
        _userManager = userManager;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<bool> Handle(AddClaimCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return false;

        var claim = new Claim(request.ClaimType, request.ClaimValue);
        var result = await _userManager.AddClaimAsync(user, claim);

        if (result.Succeeded)
        {
            await _auditLogRepository.AddAsync(new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Username = user.UserName,
                Action = AuditAction.ClaimAdded,
                EntityName = "UserClaim",
                EntityId = request.UserId.ToString(),
                NewValues = $"Type: {request.ClaimType}, Value: {request.ClaimValue}",
                Timestamp = DateTime.UtcNow
            }, cancellationToken);
        }

        return result.Succeeded;
    }
}
