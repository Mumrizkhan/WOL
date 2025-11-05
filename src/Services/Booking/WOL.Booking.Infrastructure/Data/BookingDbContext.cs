using Microsoft.EntityFrameworkCore;
using WOL.Booking.Domain.Entities;
using WOL.Booking.Domain.ValueObjects;

namespace WOL.Booking.Infrastructure.Data;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.Booking> Bookings => Set<Domain.Entities.Booking>();
    public DbSet<BANTiming> BANTimings => Set<BANTiming>();
    public DbSet<WaitingCharge> WaitingCharges => Set<WaitingCharge>();
    public DbSet<CancellationFee> CancellationFees => Set<CancellationFee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Entities.Booking>(entity =>
        {
            entity.ToTable("bookings");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookingNumber).HasColumnName("booking_number").HasMaxLength(50).IsRequired();
            entity.Property(e => e.CustomerId).HasColumnName("customer_id").IsRequired();
            entity.Property(e => e.VehicleTypeId).HasColumnName("vehicle_type_id").IsRequired();
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");
            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.Property(e => e.BookingType).HasColumnName("booking_type").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").IsRequired();
            entity.Property(e => e.TotalFare).HasColumnName("total_fare").HasColumnType("decimal(10,2)").IsRequired();
            entity.Property(e => e.DiscountAmount).HasColumnName("discount_amount").HasColumnType("decimal(10,2)");
            entity.Property(e => e.FinalFare).HasColumnName("final_fare").HasColumnType("decimal(10,2)").IsRequired();
            entity.Property(e => e.PickupDate).HasColumnName("pickup_date").IsRequired();
            entity.Property(e => e.PickupTime).HasColumnName("pickup_time").IsRequired();
            entity.Property(e => e.DriverAssignedAt).HasColumnName("driver_assigned_at");
            entity.Property(e => e.DriverAcceptedAt).HasColumnName("driver_accepted_at");
            entity.Property(e => e.DriverReachedAt).HasColumnName("driver_reached_at");
            entity.Property(e => e.LoadingStartedAt).HasColumnName("loading_started_at");
            entity.Property(e => e.InTransitAt).HasColumnName("in_transit_at");
            entity.Property(e => e.DeliveredAt).HasColumnName("delivered_at");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at");
            entity.Property(e => e.CancellationReason).HasColumnName("cancellation_reason").HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.OwnsOne(e => e.Origin, origin =>
            {
                origin.Property(o => o.Address).HasColumnName("origin_address").HasMaxLength(500).IsRequired();
                origin.Property(o => o.Latitude).HasColumnName("origin_latitude").HasColumnType("decimal(10,7)").IsRequired();
                origin.Property(o => o.Longitude).HasColumnName("origin_longitude").HasColumnType("decimal(10,7)").IsRequired();
                origin.Property(o => o.City).HasColumnName("origin_city").HasMaxLength(100).IsRequired();
            });

            entity.OwnsOne(e => e.Destination, destination =>
            {
                destination.Property(d => d.Address).HasColumnName("destination_address").HasMaxLength(500).IsRequired();
                destination.Property(d => d.Latitude).HasColumnName("destination_latitude").HasColumnType("decimal(10,7)").IsRequired();
                destination.Property(d => d.Longitude).HasColumnName("destination_longitude").HasColumnType("decimal(10,7)").IsRequired();
                destination.Property(d => d.City).HasColumnName("destination_city").HasMaxLength(100).IsRequired();
            });

            entity.OwnsOne(e => e.Cargo, cargo =>
            {
                cargo.Property(c => c.Type).HasColumnName("cargo_type").HasMaxLength(50).IsRequired();
                cargo.Property(c => c.GrossWeightKg).HasColumnName("cargo_gross_weight_kg").HasColumnType("decimal(10,2)").IsRequired();
                cargo.Property(c => c.NetWeightKg).HasColumnName("cargo_net_weight_kg").HasColumnType("decimal(10,2)");
                cargo.Property(c => c.NumberOfBoxes).HasColumnName("cargo_number_of_boxes");
                cargo.Property(c => c.Description).HasColumnName("cargo_description").HasMaxLength(500);
            });

            entity.OwnsOne(e => e.Shipper, shipper =>
            {
                shipper.Property(s => s.Name).HasColumnName("shipper_name").HasMaxLength(200).IsRequired();
                shipper.Property(s => s.Mobile).HasColumnName("shipper_mobile").HasMaxLength(20).IsRequired();
                shipper.Property(s => s.Email).HasColumnName("shipper_email").HasMaxLength(255);
            });

            entity.OwnsOne(e => e.Receiver, receiver =>
            {
                receiver.Property(r => r.Name).HasColumnName("receiver_name").HasMaxLength(200).IsRequired();
                receiver.Property(r => r.Mobile).HasColumnName("receiver_mobile").HasMaxLength(20).IsRequired();
                receiver.Property(r => r.Email).HasColumnName("receiver_email").HasMaxLength(255);
            });

            entity.HasIndex(e => e.BookingNumber).IsUnique();
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.DriverId);
            entity.HasIndex(e => e.Status);
            
            entity.Ignore(e => e.DomainEvents);
        });

        modelBuilder.ApplyConfiguration(new Configurations.BANTimingConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.WaitingChargeConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CancellationFeeConfiguration());
    }
}
