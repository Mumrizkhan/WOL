using Microsoft.EntityFrameworkCore;
using WOL.Tracking.Domain.Entities;

namespace WOL.Tracking.Infrastructure.Data;

public class TrackingDbContext : DbContext
{
    public TrackingDbContext(DbContextOptions<TrackingDbContext> options) : base(options)
    {
    }

    public DbSet<LocationHistory> LocationHistories => Set<LocationHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LocationHistory>(entity =>
        {
            entity.ToTable("location_history");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id").IsRequired();
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id").IsRequired();
            entity.Property(e => e.DriverId).HasColumnName("driver_id").IsRequired();
            entity.Property(e => e.Latitude).HasColumnName("latitude").HasColumnType("decimal(10,7)").IsRequired();
            entity.Property(e => e.Longitude).HasColumnName("longitude").HasColumnType("decimal(10,7)").IsRequired();
            entity.Property(e => e.Speed).HasColumnName("speed").HasColumnType("decimal(5,2)");
            entity.Property(e => e.Heading).HasColumnName("heading").HasColumnType("decimal(5,2)");
            entity.Property(e => e.Timestamp).HasColumnName("timestamp").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.BookingId);
            entity.HasIndex(e => e.Timestamp);
            
            entity.Ignore(e => e.DomainEvents);
        });
    }
}
