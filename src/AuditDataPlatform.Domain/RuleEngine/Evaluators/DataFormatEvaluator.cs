using System.Globalization;
using System.Text.Json;
using AuditDataPlatform.Domain.Enums;

namespace AuditDataPlatform.Domain.RuleEngine.Evaluators;

public sealed class DataFormatEvaluator: IRuleEvaluator
{
    public RuleType SupportedType => RuleType.DataFormat;

    private sealed class Parameters
    {
        public string Column { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
    }

    public IEnumerable<RuleEvaluationResult> Evaluate(RuleEvaluationContext context)
    {
        var parameters = JsonSerializer.Deserialize<Parameters>(context.Rule.ParametersJson, RuleParameterJson.Options) ?? throw new InvalidOperationException($"Rule '{context.Rule.Name}': invalid ParametersJson for DataFormat.");

        foreach (var row in context.Rows)
        {
            if (!row.Values.TryGetValue(parameters.Column, out var value) || string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            var isValid = DateTime.TryParseExact(value, parameters.Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);

            if (!isValid)
            {
                yield return new RuleEvaluationResult
                {
                    RowNumber = row.RowNumber,
                    Column = parameters.Column,
                    Message = $"Value '{value}' does not match expected format '{parameters.Format}'."
                };
            }
        }
    }
}