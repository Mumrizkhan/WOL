using Microsoft.EntityFrameworkCore;

namespace WOL.Payment.Infrastructure.Data;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.Payment> Payments => Set<Domain.Entities.Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Entities.Payment>(entity =>
        {
            entity.ToTable("payments");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id").HasMaxLength(50).IsRequired();
            entity.Property(e => e.BookingId).HasColumnName("booking_id").IsRequired();
            entity.Property(e => e.CustomerId).HasColumnName("customer_id").IsRequired();
            entity.Property(e => e.Amount).HasColumnName("amount").HasColumnType("decimal(10,2)").IsRequired();
            entity.Property(e => e.PaymentMethod).HasColumnName("payment_method").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").IsRequired();
            entity.Property(e => e.PaymentGatewayReference).HasColumnName("payment_gateway_reference").HasMaxLength(200);
            entity.Property(e => e.ProcessedAt).HasColumnName("processed_at");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.FailureReason).HasColumnName("failure_reason").HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.TransactionId).IsUnique();
            entity.HasIndex(e => e.BookingId);
            entity.HasIndex(e => e.CustomerId);
            
            entity.Ignore(e => e.DomainEvents);
        });
    }
}
