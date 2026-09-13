using System.Text.Json;
using AuditDataPlatform.Domain.Entities;
using AuditDataPlatform.Domain.Enums;
using AuditDataPlatform.Domain.RuleEngine;

namespace AuditDataPlatform.Application.RuleEngine;

public sealed class RuleEngineOrchestrator : IRuleEngineOrchestrator
{
    private readonly IReadOnlyDictionary<RuleType, IRuleEvaluator> _evaluatorsByType;

    public RuleEngineOrchestrator(IEnumerable<IRuleEvaluator> evaluators)
    {
        _evaluatorsByType = evaluators.ToDictionary(e => e.SupportedType);
    }

    public IReadOnlyList<Finding> Evaluate(RuleSet ruleSet, IReadOnlyList<ImportRow> importRows)
    {
        if (importRows.Count == 0)
        {
            return Array.Empty<Finding>();
        }

        var rowsByNumber = importRows.ToDictionary(r => r.RowNumber);
        var evaluationRows = importRows
            .Select(row => new RuleEvaluationRow
            {
                RowNumber = row.RowNumber,
                Values = ParseRowValues(row.DataJson)
            })
            .ToList();

        var findings = new List<Finding>();

        foreach (var rule in ruleSet.Rules.Where(r => r.IsActive).OrderBy(r => r.SortOrder))
        {
            if (!_evaluatorsByType.TryGetValue(rule.Type, out var evaluator))
            {
                throw new InvalidOperationException(
                    $"No evaluator registered for rule type '{rule.Type}' (rule '{rule.Name}').");
            }

            var context = new RuleEvaluationContext { Rule = rule, Rows = evaluationRows };

            foreach (var result in evaluator.Evaluate(context))
            {
                if (!rowsByNumber.TryGetValue(result.RowNumber, out var importRow))
                {
                    throw new InvalidOperationException(
                        $"Evaluator for rule '{rule.Name}' returned a finding for row {result.RowNumber}, which isn't in the evaluated set.");
                }

                findings.Add(new Finding
                {
                    ImportJobId = importRow.ImportJobId,
                    ImportRowId = importRow.Id,
                    RuleId = rule.Id,
                    Severity = rule.Severity,
                    Message = result.Message,
                    Column = result.Column
                });
            }
        }

        return findings;
    }

    private static Dictionary<string, string?> ParseRowValues(string dataJson)
    {
        using var document = JsonDocument.Parse(dataJson);
        var values = new Dictionary<string, string?>();

        foreach (var property in document.RootElement.EnumerateObject())
        {
            values[property.Name] = property.Value.ValueKind switch
            {
                JsonValueKind.Null => null,
                JsonValueKind.String => property.Value.GetString(),
                _ => property.Value.GetRawText()
            };
        }

        return values;
    }
}