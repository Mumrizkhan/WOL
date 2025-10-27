using Microsoft.EntityFrameworkCore;
using WOL.Pricing.Domain.Entities;

namespace WOL.Pricing.Infrastructure.Data;

public class PricingDbContext : DbContext
{
    public PricingDbContext(DbContextOptions<PricingDbContext> options) : base(options)
    {
    }

    public DbSet<PricingRule> PricingRules => Set<PricingRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PricingRule>(entity =>
        {
            entity.ToTable("pricing_rules");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.VehicleType).HasColumnName("vehicle_type").HasMaxLength(50).IsRequired();
            entity.Property(e => e.BasePrice).HasColumnName("base_price").HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.PricePerKm).HasColumnName("price_per_km").HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.PricePerKg).HasColumnName("price_per_kg").HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.FromCity).HasColumnName("from_city").HasMaxLength(100).IsRequired();
            entity.Property(e => e.ToCity).HasColumnName("to_city").HasMaxLength(100).IsRequired();
            entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => new { e.FromCity, e.ToCity, e.VehicleType });
            
            entity.Ignore(e => e.DomainEvents);
        });
    }
}
