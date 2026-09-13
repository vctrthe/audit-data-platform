using System.Globalization;
using System.Text.Json;
using AuditDataPlatform.Domain.Enums;

namespace AuditDataPlatform.Domain.RuleEngine.Evaluators;

public sealed class RangeThresholdEvaluator : IRuleEvaluator
{
    public RuleType SupportedType => RuleType.RangeThreshold;

    private sealed class Parameters
    {
        public string Column { get; set; } = string.Empty;
        public double? Min { get; set; }
        public double? Max { get; set; }
    }

    public IEnumerable<RuleEvaluationResult> Evaluate(RuleEvaluationContext context)
    {
        var parameters = JsonSerializer.Deserialize<Parameters>(context.Rule.ParametersJson, RuleParameterJson.Options)
            ?? throw new InvalidOperationException($"Rule '{context.Rule.Name}': invalid ParametersJson for RangeThreshold.");

        foreach (var row in context.Rows)
        {
            if (!row.Values.TryGetValue(parameters.Column, out var raw) || string.IsNullOrWhiteSpace(raw))
            {
                continue; // missing value is RequiredField's concern
            }

            if (!double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            {
                continue; // non-numeric value is DataFormat's concern
            }

            if (parameters.Min.HasValue && value < parameters.Min.Value)
            {
                yield return new RuleEvaluationResult
                {
                    RowNumber = row.RowNumber,
                    Column = parameters.Column,
                    Message = $"Value {value} is below the minimum threshold of {parameters.Min.Value}."
                };
            }
            else if (parameters.Max.HasValue && value > parameters.Max.Value)
            {
                yield return new RuleEvaluationResult
                {
                    RowNumber = row.RowNumber,
                    Column = parameters.Column,
                    Message = $"Value {value} exceeds the maximum threshold of {parameters.Max.Value}."
                };
            }
        }
    }
}