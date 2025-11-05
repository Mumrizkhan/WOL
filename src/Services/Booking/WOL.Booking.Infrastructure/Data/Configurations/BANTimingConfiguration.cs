using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WOL.Booking.Domain.Entities;

namespace WOL.Booking.Infrastructure.Data.Configurations;

public class BANTimingConfiguration : IEntityTypeConfiguration<BANTiming>
{
    public void Configure(EntityTypeBuilder<BANTiming> builder)
    {
        builder.ToTable("BANTimings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.DayOfWeek)
            .IsRequired();

        builder.Property(b => b.StartTime)
            .IsRequired();

        builder.Property(b => b.EndTime)
            .IsRequired();

        builder.Property(b => b.IsActive)
            .IsRequired();

        builder.Property(b => b.Description)
            .HasMaxLength(500);

        builder.HasIndex(b => new { b.City, b.DayOfWeek, b.IsActive });
    }
}
