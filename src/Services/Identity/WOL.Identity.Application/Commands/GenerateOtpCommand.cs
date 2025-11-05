using MediatR;
using WOL.Identity.Domain.Enums;

namespace WOL.Identity.Application.Commands;

public record GenerateOtpCommand(
    Guid UserId,
    OtpPurpose Purpose,
    string? PhoneNumber = null,
    string? Email = null) : IRequest<string>;
