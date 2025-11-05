using MediatR;
using Microsoft.AspNetCore.Identity;
using WOL.Identity.Domain.Entities;

namespace WOL.Identity.Application.Queries;

public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, IEnumerable<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserRolesQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<string>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return Enumerable.Empty<string>();

        return await _userManager.GetRolesAsync(user);
    }
}
