using System.Globalization;
using System.Text.Json;
using AuditDataPlatform.Domain.Enums;

namespace AuditDataPlatform.Domain.RuleEngine.Evaluators;

public sealed class CrossFieldConsistencyEvaluator : IRuleEvaluator
{
    public RuleType SupportedType => RuleType.CrossFieldConsistency;

    private sealed class Parameters
    {
        public string Left { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty;
        public string Right { get; set; } = string.Empty;
    }

    public IEnumerable<RuleEvaluationResult> Evaluate(RuleEvaluationContext context)
    {
        var parameters = JsonSerializer.Deserialize<Parameters>(context.Rule.ParametersJson, RuleParameterJson.Options)
            ?? throw new InvalidOperationException($"Rule '{context.Rule.Name}': invalid ParametersJson for CrossFieldConsistency.");

        foreach (var row in context.Rows)
        {
            var hasLeft = row.Values.TryGetValue(parameters.Left, out var leftRaw) && !string.IsNullOrWhiteSpace(leftRaw);
            var hasRight = row.Values.TryGetValue(parameters.Right, out var rightRaw) && !string.IsNullOrWhiteSpace(rightRaw);

            if (!hasLeft || !hasRight)
            {
                continue; // missing values are RequiredField's concern
            }

            var comparison = Compare(leftRaw!, rightRaw!);
            if (comparison is null)
            {
                continue; // not comparable as dates or numbers — skip rather than guess
            }

            var satisfied = parameters.Operator switch
            {
                "==" => comparison == 0,
                "!=" => comparison != 0,
                ">" => comparison > 0,
                ">=" => comparison >= 0,
                "<" => comparison < 0,
                "<=" => comparison <= 0,
                _ => throw new InvalidOperationException(
                    $"Rule '{context.Rule.Name}': unsupported operator '{parameters.Operator}'.")
            };

            if (!satisfied)
            {
                yield return new RuleEvaluationResult
                {
                    RowNumber = row.RowNumber,
                    Column = $"{parameters.Left},{parameters.Right}",
                    Message = $"'{parameters.Left}' ({leftRaw}) does not satisfy '{parameters.Operator}' against '{parameters.Right}' ({rightRaw})."
                };
            }
        }
    }

    private static int? Compare(string left, string right)
    {
        if (DateTime.TryParse(left, CultureInfo.InvariantCulture, DateTimeStyles.None, out var leftDate)
            && DateTime.TryParse(right, CultureInfo.InvariantCulture, DateTimeStyles.None, out var rightDate))
        {
            return leftDate.CompareTo(rightDate);
        }

        if (double.TryParse(left, NumberStyles.Float, CultureInfo.InvariantCulture, out var leftNum)
            && double.TryParse(right, NumberStyles.Float, CultureInfo.InvariantCulture, out var rightNum))
        {
            return leftNum.CompareTo(rightNum);
        }

        return null;
    }
}