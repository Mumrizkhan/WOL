using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using MassTransit;
using WOL.Identity.Domain.Entities;
using WOL.Identity.Domain.Enums;
using WOL.Shared.Messages.Events;

namespace WOL.Identity.Application.Commands;

public class AddClaimCommandHandler : IRequestHandler<AddClaimCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPublishEndpoint _publishEndpoint;

    public AddClaimCommandHandler(
        UserManager<ApplicationUser> userManager,
        IPublishEndpoint publishEndpoint)
    {
        _userManager = userManager;
        _publishEndpoint = publishEndpoint;
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
            await _publishEndpoint.Publish(new AuditLogCreatedEvent
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Username = user.UserName,
                Action = AuditAction.ClaimAdded.ToString(),
                EntityName = "UserClaim",
                EntityId = request.UserId.ToString(),
                NewValues = $"Type: {request.ClaimType}, Value: {request.ClaimValue}",
                Timestamp = DateTime.UtcNow
            }, cancellationToken);
        }

        return result.Succeeded;
    }
}
