namespace WOL.Identity.Domain.Enums;

public enum AuditAction
{
    Create = 1,
    Update = 2,
    Delete = 3,
    Login = 4,
    Logout = 5,
    PasswordChange = 6,
    PasswordReset = 7,
    RoleAssigned = 8,
    RoleRemoved = 9,
    ClaimAdded = 10,
    ClaimRemoved = 11,
    AccountLocked = 12,
    AccountUnlocked = 13,
    OtpGenerated = 14,
    OtpVerified = 15
}
