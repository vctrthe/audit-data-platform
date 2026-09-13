using System.Text.Json;
using AuditDataPlatform.Domain.Enums;

namespace AuditDataPlatform.Domain.RuleEngine.Evaluators;

public sealed class DuplicateEvaluator : IRuleEvaluator
{
    public RuleType SupportedType => RuleType.Duplicate;

    private sealed class Parameters
    {
        public string[] KeyColumns { get; set; } = Array.Empty<string>();
    }

    public IEnumerable<RuleEvaluationResult> Evaluate(RuleEvaluationContext context)
    {
        var parameters = JsonSerializer.Deserialize<Parameters>(context.Rule.ParametersJson, RuleParameterJson.Options)
            ?? throw new InvalidOperationException($"Rule '{context.Rule.Name}': invalid ParametersJson for Duplicate.");

        if (parameters.KeyColumns.Length == 0)
        {
            yield break;
        }

        // Rows where every key column is empty are excluded here — that's RequiredField's
        // job to flag, not Duplicate's; otherwise all-blank rows would falsely group together.
        var candidateRows = context.Rows
            .Where(row => parameters.KeyColumns.Any(col => row.Values.TryGetValue(col, out var v) && !string.IsNullOrWhiteSpace(v)))
            .ToList();

        var groups = candidateRows
            .GroupBy(row => string.Join(
                "\u0001",
                parameters.KeyColumns.Select(col => row.Values.TryGetValue(col, out var v) ? v ?? string.Empty : string.Empty)))
            .Where(g => g.Count() > 1);

        foreach (var group in groups)
        {
            var keyDescription = string.Join(", ", parameters.KeyColumns.Select(col =>
                $"{col}='{(group.First().Values.TryGetValue(col, out var v) ? v : "")}'"));

            foreach (var row in group)
            {
                yield return new RuleEvaluationResult
                {
                    RowNumber = row.RowNumber,
                    Column = string.Join(",", parameters.KeyColumns),
                    Message = $"Duplicate row: another row shares the same key ({keyDescription})."
                };
            }
        }
    }
}