using Microsoft.EntityFrameworkCore;
using WOL.Vehicle.Domain.Entities;

namespace WOL.Vehicle.Infrastructure.Data;

public class VehicleDbContext : DbContext
{
    public VehicleDbContext(DbContextOptions<VehicleDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.Vehicle> Vehicles => Set<Domain.Entities.Vehicle>();
    public DbSet<VehicleType> VehicleTypes => Set<VehicleType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Entities.Vehicle>(entity =>
        {
            entity.ToTable("vehicles");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id").IsRequired();
            entity.Property(e => e.VehicleTypeId).HasColumnName("vehicle_type_id").IsRequired();
            entity.Property(e => e.PlateNumber).HasColumnName("plate_number").HasMaxLength(20).IsRequired();
            entity.Property(e => e.Make).HasColumnName("make").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Model).HasColumnName("model").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Year).HasColumnName("year").IsRequired();
            entity.Property(e => e.Color).HasColumnName("color").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").IsRequired();
            entity.Property(e => e.IsAvailable).HasColumnName("is_available").IsRequired();
            entity.Property(e => e.LastMaintenanceDate).HasColumnName("last_maintenance_date");
            entity.Property(e => e.NextMaintenanceDate).HasColumnName("next_maintenance_date");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.PlateNumber).IsUnique();
            entity.HasIndex(e => e.OwnerId);
            entity.HasIndex(e => e.VehicleTypeId);
            
            entity.Ignore(e => e.DomainEvents);
        });

        modelBuilder.Entity<VehicleType>(entity =>
        {
            entity.ToTable("vehicle_types");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.NameAr).HasColumnName("name_ar").HasMaxLength(100).IsRequired();
            entity.Property(e => e.LoadCapacityKg).HasColumnName("load_capacity_kg").HasColumnType("decimal(10,2)").IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.Ignore(e => e.DomainEvents);
        });
    }
}
