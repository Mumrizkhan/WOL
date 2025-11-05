using WOL.Identity.Domain.Entities;
using WOL.Identity.Domain.Enums;
using WOL.Identity.Domain.Repositories;

namespace WOL.Identity.Infrastructure.Services;

public interface IOtpService
{
    Task<OtpCode> GenerateOtpAsync(Guid userId, OtpPurpose purpose, string? phoneNumber = null, string? email = null, int expiryMinutes = 10);
    Task<bool> ValidateOtpAsync(Guid userId, string code, OtpPurpose purpose);
    Task InvalidateUserOtpsAsync(Guid userId, OtpPurpose purpose);
}

public class OtpService : IOtpService
{
    private readonly IOtpRepository _otpRepository;
    private readonly Random _random = new();

    public OtpService(IOtpRepository otpRepository)
    {
        _otpRepository = otpRepository;
    }

    public async Task<OtpCode> GenerateOtpAsync(Guid userId, OtpPurpose purpose, string? phoneNumber = null, string? email = null, int expiryMinutes = 10)
    {
        await InvalidateUserOtpsAsync(userId, purpose);

        var code = GenerateRandomCode(6);
        
        var otpCode = new OtpCode
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Code = code,
            Purpose = purpose,
            PhoneNumber = phoneNumber,
            Email = email,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow,
            AttemptCount = 0
        };

        await _otpRepository.AddAsync(otpCode);
        
        return otpCode;
    }

    public async Task<bool> ValidateOtpAsync(Guid userId, string code, OtpPurpose purpose)
    {
        var otp = await _otpRepository.GetValidOtpAsync(userId, code, purpose);
        
        if (otp == null)
        {
            var latestOtp = await _otpRepository.GetLatestOtpAsync(userId, purpose);
            if (latestOtp != null && !latestOtp.IsUsed)
            {
                latestOtp.IncrementAttempt();
                await _otpRepository.UpdateAsync(latestOtp);
            }
            return false;
        }

        if (!otp.IsValid())
        {
            return false;
        }

        otp.MarkAsUsed();
        await _otpRepository.UpdateAsync(otp);
        
        return true;
    }

    public async Task InvalidateUserOtpsAsync(Guid userId, OtpPurpose purpose)
    {
        await _otpRepository.InvalidateAllUserOtpsAsync(userId, purpose);
    }

    private string GenerateRandomCode(int length)
    {
        const string chars = "0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}
