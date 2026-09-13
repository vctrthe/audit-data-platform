using System.Text.Json;
using System.Text.RegularExpressions;
using AuditDataPlatform.Domain.Enums;

namespace AuditDataPlatform.Domain.RuleEngine.Evaluators;

public sealed class PatternMatchEvaluator : IRuleEvaluator
{
    public RuleType SupportedType => RuleType.PatternMatch;

    private sealed class Parameters
    {
        public string Column { get; set; } = string.Empty;
        public string Regex { get; set; } = string.Empty;
        public string Mode { get; set; } = "mustMatch"; // "mustMatch" | "mustNotMatch"
    }

    public IEnumerable<RuleEvaluationResult> Evaluate(RuleEvaluationContext context)
    {
        var parameters = JsonSerializer.Deserialize<Parameters>(context.Rule.ParametersJson, RuleParameterJson.Options)
            ?? throw new InvalidOperationException($"Rule '{context.Rule.Name}': invalid ParametersJson for PatternMatch.");

        var regex = new Regex(parameters.Regex, RegexOptions.Compiled);

        foreach (var row in context.Rows)
        {
            if (!row.Values.TryGetValue(parameters.Column, out var value) || string.IsNullOrWhiteSpace(value))
            {
                continue; // missing value is RequiredField's concern
            }

            var isMatch = regex.IsMatch(value);
            var violated = parameters.Mode switch
            {
                "mustMatch" => !isMatch,
                "mustNotMatch" => isMatch,
                _ => throw new InvalidOperationException(
                    $"Rule '{context.Rule.Name}': unsupported PatternMatch mode '{parameters.Mode}'.")
            };

            if (violated)
            {
                yield return new RuleEvaluationResult
                {
                    RowNumber = row.RowNumber,
                    Column = parameters.Column,
                    Message = parameters.Mode == "mustMatch"
                        ? $"Value '{value}' does not match required pattern '{parameters.Regex}'."
                        : $"Value '{value}' matches forbidden pattern '{parameters.Regex}'."
                };
            }
        }
    }
}