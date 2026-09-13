using AuditDataPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuditDataPlatform.Infrastructure.Persistence.Configurations;

public sealed class RuleSetConfiguration : IEntityTypeConfiguration<RuleSet>
{
    public void Configure(EntityTypeBuilder<RuleSet> builder)
    {
        builder.ToTable("RuleSets");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.HasMany(x => x.Rules)
            .WithOne(x => x.RuleSet)
            .HasForeignKey(x => x.RuleSetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.ImportJobs)
            .WithOne(x => x.RuleSet)
            .HasForeignKey(x => x.RuleSetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}