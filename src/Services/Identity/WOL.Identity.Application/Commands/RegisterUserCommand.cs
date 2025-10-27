using MediatR;
using WOL.Identity.Domain.Enums;

namespace WOL.Identity.Application.Commands;

public record RegisterUserCommand : IRequest<RegisterUserResponse>
{
    public UserType UserType { get; init; }
    public string MobileNumber { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? IqamaNumber { get; init; }
    public string? CompanyName { get; init; }
}

public record RegisterUserResponse
{
    public Guid UserId { get; init; }
    public string MobileNumber { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}
