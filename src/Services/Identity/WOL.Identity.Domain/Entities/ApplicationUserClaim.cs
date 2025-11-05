using Microsoft.AspNetCore.Identity;

namespace WOL.Identity.Domain.Entities;

public class ApplicationUserClaim : IdentityUserClaim<Guid>
{
    public virtual ApplicationUser User { get; set; } = null!;
}
