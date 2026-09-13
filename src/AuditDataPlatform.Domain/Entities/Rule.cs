using AuditDataPlatform.Domain.Enums;

namespace AuditDataPlatform.Domain.Entities;

public class Rule
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required Guid RuleSetId { get; init; }
    public RuleSet? RuleSet { get; init; }

    public required string Name { get; set; }
    public required RuleType Type { get; set; }
    public required Severity Severity { get; set; }
    public bool IsActive { get; set; } = true;
    public required string ParametersJson { get; set; }
    public int SortOrder { get; set; }
}
