using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WOL.Identity.Domain.Entities;

namespace WOL.Identity.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<
    ApplicationUser,
    ApplicationRole,
    Guid,
    ApplicationUserClaim,
    ApplicationUserRole,
    IdentityUserLogin<Guid>,
    ApplicationRoleClaim,
    IdentityUserToken<Guid>>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("identity");

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.IqamaNumber).HasMaxLength(50);
            entity.Property(e => e.IdNumber).HasMaxLength(50);
            entity.Property(e => e.CommercialRegistration).HasMaxLength(100);
            entity.Property(e => e.PreferredLanguage).HasMaxLength(10).HasDefaultValue("en");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.HasIndex(e => e.PhoneNumber).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.IqamaNumber);
            entity.HasIndex(e => e.CommercialRegistration);
        });

        builder.Entity<ApplicationRole>(entity =>
        {
            entity.ToTable("Roles");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        builder.Entity<ApplicationUserRole>(entity =>
        {
            entity.ToTable("UserRoles");
            
            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
        });

        builder.Entity<ApplicationUserClaim>(entity =>
        {
            entity.ToTable("UserClaims");
            
            entity.HasOne(uc => uc.User)
                .WithMany(u => u.Claims)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();
        });

        builder.Entity<ApplicationRoleClaim>(entity =>
        {
            entity.ToTable("RoleClaims");
            
            entity.HasOne(rc => rc.Role)
                .WithMany(r => r.RoleClaims)
                .HasForeignKey(rc => rc.RoleId)
                .IsRequired();
        });

        builder.Entity<IdentityUserLogin<Guid>>(entity =>
        {
            entity.ToTable("UserLogins");
        });

        builder.Entity<IdentityUserToken<Guid>>(entity =>
        {
            entity.ToTable("UserTokens");
        });

        builder.Entity<OtpCode>(entity =>
        {
            entity.ToTable("OtpCodes");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Code).HasMaxLength(10).IsRequired();
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.HasOne(o => o.User)
                .WithMany(u => u.OtpCodes)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.UserId, e.Purpose, e.IsUsed });
            entity.HasIndex(e => e.ExpiresAt);
        });

        builder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLogs");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Username).HasMaxLength(256);
            entity.Property(e => e.EntityName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.EntityId).HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.HasOne(a => a.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Action);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => new { e.EntityName, e.EntityId });
        });

        SeedRoles(builder);
    }

    private void SeedRoles(ModelBuilder builder)
    {
        var roles = new[]
        {
            new ApplicationRole
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR",
                Description = "Full system access",
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationRole
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Customer",
                NormalizedName = "CUSTOMER",
                Description = "Individual customer access",
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationRole
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Supplier",
                NormalizedName = "SUPPLIER",
                Description = "Commercial supplier access",
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationRole
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Driver",
                NormalizedName = "DRIVER",
                Description = "Service provider driver access",
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationRole
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "FleetManager",
                NormalizedName = "FLEETMANAGER",
                Description = "Fleet owner management access",
                CreatedAt = DateTime.UtcNow
            }
        };

        builder.Entity<ApplicationRole>().HasData(roles);
    }
}
