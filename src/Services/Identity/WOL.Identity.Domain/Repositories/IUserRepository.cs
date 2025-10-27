using WOL.Shared.Common.Application;
using WOL.Identity.Domain.Entities;

namespace WOL.Identity.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByMobileNumberAsync(string mobileNumber, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByMobileNumberAsync(string mobileNumber, CancellationToken cancellationToken = default);
}
