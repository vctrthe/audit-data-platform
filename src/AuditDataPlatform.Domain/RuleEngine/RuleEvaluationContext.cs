using AuditDataPlatform.Domain.Entities;

namespace AuditDataPlatform.Domain.RuleEngine;

public sealed class RuleEvaluationContext
{
    public required Rule Rule { get; init; }
    public required IReadOnlyList<RuleEvaluationRow> Rows { get; init; }
}

public sealed class RuleEvaluationRow
{
    public required int RowNumber { get; init; }
    public required IReadOnlyDictionary<string, string?> Values { get; init; }
}
