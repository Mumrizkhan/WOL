using Microsoft.EntityFrameworkCore;
using WOL.Analytics.Domain.Entities;

namespace WOL.Analytics.Infrastructure.Data;

public class AnalyticsDbContext : DbContext
{
    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options)
    {
    }

    public DbSet<AnalyticsEvent> AnalyticsEvents => Set<AnalyticsEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AnalyticsEvent>(entity =>
        {
            entity.ToTable("analytics_events");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EventType).HasColumnName("event_type").HasMaxLength(100).IsRequired();
            entity.Property(e => e.EventCategory).HasColumnName("event_category").HasMaxLength(100).IsRequired();
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.EntityId).HasColumnName("entity_id").HasMaxLength(100);
            entity.Property(e => e.EntityType).HasColumnName("entity_type").HasMaxLength(50);
            entity.Property(e => e.Metadata).HasColumnName("metadata").HasColumnType("jsonb").IsRequired();
            entity.Property(e => e.EventTimestamp).HasColumnName("event_timestamp").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.EventType);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.EventTimestamp);
            
            entity.Ignore(e => e.DomainEvents);
        });
    }
}
