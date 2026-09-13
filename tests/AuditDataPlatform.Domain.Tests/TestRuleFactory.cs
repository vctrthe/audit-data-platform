using AuditDataPlatform.Domain.Entities;
using AuditDataPlatform.Domain.Enums;
using AuditDataPlatform.Domain.RuleEngine;

namespace AuditDataPlatform.Domain.Tests;

internal static class TestRuleFactory
{
    public static Rule CreateRule(RuleType type, string parametersJson, Severity severity = Severity.Error) => new()
    {
        RuleSetId = Guid.NewGuid(),
        Name = $"Test-{type}",
        Type = type,
        Severity = severity,
        ParametersJson = parametersJson
    };

    public static RuleEvaluationContext CreateContext(Rule rule, params (int RowNumber, Dictionary<string, string?> Values)[] rows) => new()
    {
        Rule = rule,
        Rows = rows.Select(r => new RuleEvaluationRow { RowNumber = r.RowNumber, Values = r.Values }).ToList()
    };
}