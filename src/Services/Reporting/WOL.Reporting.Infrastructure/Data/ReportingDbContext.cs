using Microsoft.EntityFrameworkCore;
using WOL.Reporting.Domain.Entities;

namespace WOL.Reporting.Infrastructure.Data;

public class ReportingDbContext : DbContext
{
    public ReportingDbContext(DbContextOptions<ReportingDbContext> options) : base(options)
    {
    }

    public DbSet<Report> Reports => Set<Report>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Report>(entity =>
        {
            entity.ToTable("reports");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ReportType).HasColumnName("report_type").HasMaxLength(100).IsRequired();
            entity.Property(e => e.ReportName).HasColumnName("report_name").HasMaxLength(200).IsRequired();
            entity.Property(e => e.RequestedBy).HasColumnName("requested_by");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
            entity.Property(e => e.FilePath).HasColumnName("file_path").HasMaxLength(500);
            entity.Property(e => e.GeneratedAt).HasColumnName("generated_at");
            entity.Property(e => e.Parameters).HasColumnName("parameters").HasColumnType("jsonb").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.RequestedBy);
            entity.HasIndex(e => e.Status);
            
            entity.Ignore(e => e.DomainEvents);
        });
    }
}
