using MediatR;
using System.Security.Claims;

namespace WOL.Identity.Application.Queries;

public record GetUserClaimsQuery(Guid UserId) : IRequest<IEnumerable<Claim>>;
