using System.Text.Json;
using AuditDataPlatform.Domain.Enums;

namespace AuditDataPlatform.Domain.RuleEngine.Evaluators;

public sealed class RequiredFieldEvaluator : IRuleEvaluator
{
    public RuleType SupportedType => RuleType.RequiredField;

    private sealed class Parameters
    {
        public string Column { get; set; } = string.Empty;
    }

    public IEnumerable<RuleEvaluationResult> Evaluate(RuleEvaluationContext context)
    {
        var parameters = JsonSerializer.Deserialize<Parameters>(context.Rule.ParametersJson, RuleParameterJson.Options)
            ?? throw new InvalidOperationException($"Rule '{context.Rule.Name}': invalid ParametersJson for RequiredField.");

        foreach (var row in context.Rows)
        {
            var hasValue = row.Values.TryGetValue(parameters.Column, out var value) && !string.IsNullOrWhiteSpace(value);
            if (!hasValue)
            {
                yield return new RuleEvaluationResult
                {
                    RowNumber = row.RowNumber,
                    Column = parameters.Column,
                    Message = $"Column '{parameters.Column}' is required but empty or missing."
                };
            }
        }
    }
}