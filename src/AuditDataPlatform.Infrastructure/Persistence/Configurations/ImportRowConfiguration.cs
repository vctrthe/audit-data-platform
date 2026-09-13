using AuditDataPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuditDataPlatform.Infrastructure.Persistence.Configurations;

public sealed class ImportRowConfiguration : IEntityTypeConfiguration<ImportRow>
{
    public void Configure(EntityTypeBuilder<ImportRow> builder)
    {
        builder.ToTable("ImportRows");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DataJson)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.HasIndex(x => new { x.ImportJobId, x.RowNumber })
            .IsUnique();

        builder.HasMany(x => x.Findings)
            .WithOne(x => x.ImportRow)
            .HasForeignKey(x => x.ImportRowId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}