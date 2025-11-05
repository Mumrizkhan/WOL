using Microsoft.AspNetCore.Identity;

namespace WOL.Identity.Domain.Entities;

public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
{
    public virtual ApplicationRole Role { get; set; } = null!;
}
