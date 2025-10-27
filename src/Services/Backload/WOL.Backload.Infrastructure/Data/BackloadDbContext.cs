using Microsoft.EntityFrameworkCore;
using WOL.Backload.Domain.Entities;

namespace WOL.Backload.Infrastructure.Data;

public class BackloadDbContext : DbContext
{
    public BackloadDbContext(DbContextOptions<BackloadDbContext> options) : base(options)
    {
    }

    public DbSet<BackloadOpportunity> BackloadOpportunities => Set<BackloadOpportunity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BackloadOpportunity>(entity =>
        {
            entity.ToTable("backload_opportunities");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id").IsRequired();
            entity.Property(e => e.DriverId).HasColumnName("driver_id").IsRequired();
            entity.Property(e => e.FromCity).HasColumnName("from_city").HasMaxLength(100).IsRequired();
            entity.Property(e => e.ToCity).HasColumnName("to_city").HasMaxLength(100).IsRequired();
            entity.Property(e => e.AvailableFrom).HasColumnName("available_from").IsRequired();
            entity.Property(e => e.AvailableTo).HasColumnName("available_to").IsRequired();
            entity.Property(e => e.AvailableCapacity).HasColumnName("available_capacity").HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
            entity.Property(e => e.MatchedBookingId).HasColumnName("matched_booking_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => new { e.FromCity, e.ToCity, e.Status });
            entity.HasIndex(e => e.DriverId);
            
            entity.Ignore(e => e.DomainEvents);
        });
    }
}
