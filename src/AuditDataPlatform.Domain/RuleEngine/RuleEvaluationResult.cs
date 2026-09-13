namespace AuditDataPlatform.Domain.RuleEngine;

public sealed class RuleEvaluationResult
{
    public required int RowNumber { get; init; }
    public required string Message { get; init; }
    public string? Column { get; init; }
}
