using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WOL.Booking.Domain.Entities;

namespace WOL.Booking.Infrastructure.Data.Configurations;

public class CancellationFeeConfiguration : IEntityTypeConfiguration<CancellationFee>
{
    public void Configure(EntityTypeBuilder<CancellationFee> builder)
    {
        builder.ToTable("CancellationFees");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.BookingId)
            .IsRequired();

        builder.Property(c => c.Reason)
            .IsRequired();

        builder.Property(c => c.CancelledBy)
            .IsRequired();

        builder.Property(c => c.ChargedTo)
            .IsRequired();

        builder.Property(c => c.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.CancelledAt)
            .IsRequired();

        builder.Property(c => c.IsPaid)
            .IsRequired();

        builder.Property(c => c.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(c => c.BookingId);
        builder.HasIndex(c => new { c.IsPaid, c.ChargedTo });
    }
}
