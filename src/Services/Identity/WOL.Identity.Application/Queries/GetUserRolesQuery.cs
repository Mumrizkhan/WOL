using MediatR;

namespace WOL.Identity.Application.Queries;

public record GetUserRolesQuery(Guid UserId) : IRequest<IEnumerable<string>>;
