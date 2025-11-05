using MediatR;
using WOL.Identity.Domain.Enums;

namespace WOL.Identity.Application.Commands;

public record VerifyOtpCommand(Guid UserId, string Code, OtpPurpose Purpose) : IRequest<bool>;
