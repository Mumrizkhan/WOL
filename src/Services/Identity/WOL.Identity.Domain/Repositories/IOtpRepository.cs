using WOL.Identity.Domain.Entities;
using WOL.Identity.Domain.Enums;

namespace WOL.Identity.Domain.Repositories;

public interface IOtpRepository
{
    Task<OtpCode?> GetValidOtpAsync(Guid userId, string code, OtpPurpose purpose, CancellationToken cancellationToken = default);
    Task<OtpCode?> GetLatestOtpAsync(Guid userId, OtpPurpose purpose, CancellationToken cancellationToken = default);
    Task AddAsync(OtpCode otpCode, CancellationToken cancellationToken = default);
    Task UpdateAsync(OtpCode otpCode, CancellationToken cancellationToken = default);
    Task InvalidateAllUserOtpsAsync(Guid userId, OtpPurpose purpose, CancellationToken cancellationToken = default);
}
