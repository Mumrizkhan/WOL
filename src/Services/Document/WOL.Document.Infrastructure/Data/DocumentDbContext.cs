using Microsoft.EntityFrameworkCore;

namespace WOL.Document.Infrastructure.Data;

public class DocumentDbContext : DbContext
{
    public DocumentDbContext(DbContextOptions<DocumentDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.Document> Documents => Set<Domain.Entities.Document>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Entities.Document>(entity =>
        {
            entity.ToTable("documents");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.DocumentType).HasColumnName("document_type").HasMaxLength(50).IsRequired();
            entity.Property(e => e.DocumentNumber).HasColumnName("document_number").HasMaxLength(100).IsRequired();
            entity.Property(e => e.FilePath).HasColumnName("file_path").HasMaxLength(500).IsRequired();
            entity.Property(e => e.IssueDate).HasColumnName("issue_date").IsRequired();
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date").IsRequired();
            entity.Property(e => e.IsVerified).HasColumnName("is_verified").IsRequired();
            entity.Property(e => e.VerifiedAt).HasColumnName("verified_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.ExpiryDate);
            
            entity.Ignore(e => e.DomainEvents);
        });
    }
}
