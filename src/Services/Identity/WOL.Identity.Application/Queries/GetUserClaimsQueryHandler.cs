using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WOL.Identity.Domain.Entities;

namespace WOL.Identity.Application.Queries;

public class GetUserClaimsQueryHandler : IRequestHandler<GetUserClaimsQuery, IEnumerable<Claim>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserClaimsQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<Claim>> Handle(GetUserClaimsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return Enumerable.Empty<Claim>();

        return await _userManager.GetClaimsAsync(user);
    }
}
