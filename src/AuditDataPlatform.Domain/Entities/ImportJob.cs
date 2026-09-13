using AuditDataPlatform.Domain.Enums;

namespace AuditDataPlatform.Domain.Entities;

public class ImportJob
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required Guid RuleSetId { get; init; }
    public RuleSet? RuleSet { get; init; }

    public required string OriginalFileName { get; set; }
    public required string SourceFormat { get; set; }
    public ImportStatus Status { get; set; } = ImportStatus.Pending;

    public int TotalRows { get; set; }
    public int ErrorCount { get; set; }
    public int WarningCount { get; set; }

    public DateTimeOffset UploadedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAt { get; set; }
    public string? FailureReason { get; set; }

    public ICollection<ImportRow> ImportRows { get; init; } = new List<ImportRow>();
    public ICollection<Finding> Findings { get; init; } = new List<Finding>();
}
