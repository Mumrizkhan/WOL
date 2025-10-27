using Microsoft.EntityFrameworkCore;
using WOL.Compliance.Domain.Entities;

namespace WOL.Compliance.Infrastructure.Data;

public class ComplianceDbContext : DbContext
{
    public ComplianceDbContext(DbContextOptions<ComplianceDbContext> options) : base(options)
    {
    }

    public DbSet<ComplianceCheck> ComplianceChecks => Set<ComplianceCheck>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ComplianceCheck>(entity =>
        {
            entity.ToTable("compliance_checks");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EntityId).HasColumnName("entity_id").IsRequired();
            entity.Property(e => e.EntityType).HasColumnName("entity_type").HasMaxLength(50).IsRequired();
            entity.Property(e => e.CheckType).HasColumnName("check_type").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
            entity.Property(e => e.Result).HasColumnName("result").HasMaxLength(50);
            entity.Property(e => e.CheckedAt).HasColumnName("checked_at");
            entity.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.EntityId);
            entity.HasIndex(e => e.Status);
            
            entity.Ignore(e => e.DomainEvents);
        });
    }
}
