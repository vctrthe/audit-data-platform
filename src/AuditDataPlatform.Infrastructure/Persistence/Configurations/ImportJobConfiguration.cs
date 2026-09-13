using AuditDataPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuditDataPlatform.Infrastructure.Persistence.Configurations;

public sealed class ImportJobConfiguration : IEntityTypeConfiguration<ImportJob>
{
    public void Configure(EntityTypeBuilder<ImportJob> builder)
    {
        builder.ToTable("ImportJobs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OriginalFileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.SourceFormat)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.FailureReason)
            .HasMaxLength(2000);

        builder.HasMany(x => x.ImportRows)
            .WithOne(x => x.ImportJob)
            .HasForeignKey(x => x.ImportJobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Findings)
            .WithOne(x => x.ImportJob)
            .HasForeignKey(x => x.ImportJobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}