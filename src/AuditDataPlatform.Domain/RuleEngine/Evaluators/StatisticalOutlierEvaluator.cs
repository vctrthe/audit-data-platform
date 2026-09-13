using System.Globalization;
using System.Text.Json;
using AuditDataPlatform.Domain.Enums;

namespace AuditDataPlatform.Domain.RuleEngine.Evaluators;

public sealed class StatisticalOutlierEvaluator : IRuleEvaluator
{
    public RuleType SupportedType => RuleType.StatisticalOutlier;

    private sealed class Parameters
    {
        public string Column { get; set; } = string.Empty;
        public double ZScoreThreshold { get; set; } = 3.0;
    }

    public IEnumerable<RuleEvaluationResult> Evaluate(RuleEvaluationContext context)
    {
        var parameters = JsonSerializer.Deserialize<Parameters>(context.Rule.ParametersJson, RuleParameterJson.Options)
            ?? throw new InvalidOperationException($"Rule '{context.Rule.Name}': invalid ParametersJson for StatisticalOutlier.");

        var numericRows = new List<(int RowNumber, double Value)>();
        foreach (var row in context.Rows)
        {
            if (row.Values.TryGetValue(parameters.Column, out var raw)
                && !string.IsNullOrWhiteSpace(raw)
                && double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            {
                numericRows.Add((row.RowNumber, value));
            }
        }

        if (numericRows.Count < 2)
        {
            yield break; // not enough data points for a meaningful standard deviation
        }

        var mean = numericRows.Average(r => r.Value);
        var variance = numericRows.Sum(r => Math.Pow(r.Value - mean, 2)) / (numericRows.Count - 1);
        var stdDev = Math.Sqrt(variance);

        if (stdDev == 0)
        {
            yield break; // every value identical; no outliers possible
        }

        foreach (var (rowNumber, value) in numericRows)
        {
            var zScore = (value - mean) / stdDev;
            if (Math.Abs(zScore) > parameters.ZScoreThreshold)
            {
                yield return new RuleEvaluationResult
                {
                    RowNumber = rowNumber,
                    Column = parameters.Column,
                    Message = $"Value {value} is a statistical outlier (z-score {zScore:F2}, threshold {parameters.ZScoreThreshold})."
                };
            }
        }
    }
}