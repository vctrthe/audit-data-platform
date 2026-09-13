using AuditDataPlatform.Domain.Enums;

namespace AuditDataPlatform.Domain.Entities;

public class Finding
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required Guid ImportJobId { get; init; }
    public ImportJob? ImportJob { get; init; }

    public required Guid ImportRowId { get; init; }
    public ImportRow? ImportRow { get; init; }

    public required int RowNumber { get; init; }

    public required Guid RuleId { get; init; }
    public Rule? Rule { get; init; }

    public required Severity Severity { get; set; }
    public required string Message { get; set; }
    public string? Column { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
