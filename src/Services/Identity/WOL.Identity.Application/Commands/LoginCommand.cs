using MediatR;

namespace WOL.Identity.Application.Commands;

public record LoginCommand : IRequest<LoginResponse>
{
    public string MobileNumber { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public record LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public Guid UserId { get; init; }
    public string UserType { get; init; } = string.Empty;
}
