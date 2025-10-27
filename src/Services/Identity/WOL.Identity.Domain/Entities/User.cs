using WOL.Shared.Common.Domain;
using WOL.Identity.Domain.Enums;
using WOL.Identity.Domain.Events;

namespace WOL.Identity.Domain.Entities;

public class User : BaseEntity
{
    public UserType UserType { get; private set; }
    public string MobileNumber { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public string? IqamaNumber { get; private set; }
    public string? IdNumber { get; private set; }
    public string? CommercialRegistration { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? CompanyName { get; private set; }
    public string PreferredLanguage { get; private set; } = "en";
    public bool IsActive { get; private set; }
    public bool IsVerified { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private User() { }

    public static User Create(
        UserType userType,
        string mobileNumber,
        string passwordHash,
        string firstName,
        string lastName,
        string? email = null,
        string? iqamaNumber = null,
        string? companyName = null)
    {
        var user = new User
        {
            UserType = userType,
            MobileNumber = mobileNumber,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            IqamaNumber = iqamaNumber,
            CompanyName = companyName,
            PreferredLanguage = "en",
            IsActive = true,
            IsVerified = false
        };

        user.AddDomainEvent(new UserCreatedEvent
        {
            UserId = user.Id,
            UserType = user.UserType,
            MobileNumber = user.MobileNumber,
            Email = user.Email,
            OccurredOn = DateTime.UtcNow
        });

        return user;
    }

    public void Verify()
    {
        IsVerified = true;
        SetUpdatedAt();
    }

    public void UpdateProfile(string firstName, string lastName, string? email, string preferredLanguage)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PreferredLanguage = preferredLanguage;
        SetUpdatedAt();
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        SetUpdatedAt();
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }
}
