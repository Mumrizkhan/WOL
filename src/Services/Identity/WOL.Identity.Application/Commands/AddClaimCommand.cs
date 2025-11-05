using MediatR;

namespace WOL.Identity.Application.Commands;

public record AddClaimCommand(Guid UserId, string ClaimType, string ClaimValue) : IRequest<bool>;
