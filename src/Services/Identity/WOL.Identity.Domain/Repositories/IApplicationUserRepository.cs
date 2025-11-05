using WOL.Identity.Domain.Entities;

namespace WOL.Identity.Domain.Repositories;

public interface IApplicationUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<ApplicationUser>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ApplicationUser>> GetByRoleAsync(string roleName, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
}
