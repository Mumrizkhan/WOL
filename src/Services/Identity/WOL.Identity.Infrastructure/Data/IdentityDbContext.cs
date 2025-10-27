using Microsoft.EntityFrameworkCore;
using WOL.Identity.Domain.Entities;

namespace WOL.Identity.Infrastructure.Data;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserType).HasColumnName("user_type").IsRequired();
            entity.Property(e => e.MobileNumber).HasColumnName("mobile_number").HasMaxLength(20).IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(e => e.IqamaNumber).HasColumnName("iqama_number").HasMaxLength(50);
            entity.Property(e => e.IdNumber).HasColumnName("id_number").HasMaxLength(50);
            entity.Property(e => e.CommercialRegistration).HasColumnName("commercial_registration").HasMaxLength(50);
            entity.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.CompanyName).HasColumnName("company_name").HasMaxLength(200);
            entity.Property(e => e.PreferredLanguage).HasColumnName("preferred_language").HasMaxLength(10).IsRequired();
            entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired();
            entity.Property(e => e.IsVerified).HasColumnName("is_verified").IsRequired();
            entity.Property(e => e.LastLoginAt).HasColumnName("last_login_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.MobileNumber).IsUnique();
            entity.HasIndex(e => e.Email);
            
            entity.Ignore(e => e.DomainEvents);
        });
    }
}
