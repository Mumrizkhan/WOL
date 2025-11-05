using Microsoft.EntityFrameworkCore;
using WOL.Identity.Domain.Entities;
using WOL.Identity.Domain.Enums;
using WOL.Identity.Domain.Repositories;
using WOL.Identity.Infrastructure.Data;

namespace WOL.Identity.Infrastructure.Repositories;

public class OtpRepository : IOtpRepository
{
    private readonly ApplicationDbContext _context;

    public OtpRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OtpCode?> GetValidOtpAsync(Guid userId, string code, OtpPurpose purpose, CancellationToken cancellationToken = default)
    {
        return await _context.OtpCodes
            .FirstOrDefaultAsync(o => 
                o.UserId == userId && 
                o.Code == code && 
                o.Purpose == purpose && 
                !o.IsUsed && 
                o.ExpiresAt > DateTime.UtcNow &&
                o.AttemptCount < 5,
                cancellationToken);
    }

    public async Task<OtpCode?> GetLatestOtpAsync(Guid userId, OtpPurpose purpose, CancellationToken cancellationToken = default)
    {
        return await _context.OtpCodes
            .Where(o => o.UserId == userId && o.Purpose == purpose)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(OtpCode otpCode, CancellationToken cancellationToken = default)
    {
        await _context.OtpCodes.AddAsync(otpCode, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(OtpCode otpCode, CancellationToken cancellationToken = default)
    {
        _context.OtpCodes.Update(otpCode);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task InvalidateAllUserOtpsAsync(Guid userId, OtpPurpose purpose, CancellationToken cancellationToken = default)
    {
        var otps = await _context.OtpCodes
            .Where(o => o.UserId == userId && o.Purpose == purpose && !o.IsUsed)
            .ToListAsync(cancellationToken);

        foreach (var otp in otps)
        {
            otp.MarkAsUsed();
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
