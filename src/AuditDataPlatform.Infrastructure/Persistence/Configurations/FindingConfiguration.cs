using AuditDataPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuditDataPlatform.Infrastructure.Persistence.Configurations;

public sealed class FindingConfiguration : IEntityTypeConfiguration<Finding>
{
    public void Configure(EntityTypeBuilder<Finding> builder)
    {
        builder.ToTable("Findings");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Severity)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.Column)
            .HasMaxLength(200);

        builder.HasOne(x => x.Rule)
            .WithMany()
            .HasForeignKey(x => x.RuleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ImportJobId);
    }
}