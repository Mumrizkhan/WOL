using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WOL.Booking.Domain.Entities;

namespace WOL.Booking.Infrastructure.Data.Configurations;

public class WaitingChargeConfiguration : IEntityTypeConfiguration<WaitingCharge>
{
    public void Configure(EntityTypeBuilder<WaitingCharge> builder)
    {
        builder.ToTable("WaitingCharges");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.BookingId)
            .IsRequired();

        builder.Property(w => w.StartTime)
            .IsRequired();

        builder.Property(w => w.EndTime);

        builder.Property(w => w.HoursCharged)
            .IsRequired();

        builder.Property(w => w.HourlyRate)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(w => w.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(w => w.IsPaid)
            .IsRequired();

        builder.HasIndex(w => w.BookingId);
        builder.HasIndex(w => new { w.IsPaid, w.BookingId });
    }
}
