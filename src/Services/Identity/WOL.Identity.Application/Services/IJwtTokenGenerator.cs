using WOL.Identity.Domain.Entities;

namespace WOL.Identity.Application.Services;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
