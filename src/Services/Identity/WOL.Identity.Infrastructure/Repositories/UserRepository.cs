using Microsoft.EntityFrameworkCore;
using WOL.Identity.Domain.Entities;
using WOL.Identity.Domain.Repositories;
using WOL.Identity.Infrastructure.Data;

namespace WOL.Identity.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _context;

    public UserRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }

    public async Task<User> AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User entity, CancellationToken cancellationToken = default)
    {
        _context.Users.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<User?> GetByMobileNumberAsync(string mobileNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.MobileNumber == mobileNumber, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByMobileNumberAsync(string mobileNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.MobileNumber == mobileNumber, cancellationToken);
    }
}
