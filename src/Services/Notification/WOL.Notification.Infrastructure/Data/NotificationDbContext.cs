using Microsoft.EntityFrameworkCore;

namespace WOL.Notification.Infrastructure.Data;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.Notification> Notifications => Set<Domain.Entities.Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Entities.Notification>(entity =>
        {
            entity.ToTable("notifications");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Body).HasColumnName("body").HasMaxLength(1000).IsRequired();
            entity.Property(e => e.Type).HasColumnName("type").HasMaxLength(50).IsRequired();
            entity.Property(e => e.IsRead).HasColumnName("is_read").IsRequired();
            entity.Property(e => e.ReadAt).HasColumnName("read_at");
            entity.Property(e => e.Data).HasColumnName("data").HasColumnType("jsonb");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IsRead);
            
            entity.Ignore(e => e.DomainEvents);
        });
    }
}
