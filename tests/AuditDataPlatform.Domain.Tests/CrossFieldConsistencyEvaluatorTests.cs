using AuditDataPlatform.Domain.Enums;
using AuditDataPlatform.Domain.RuleEngine.Evaluators;
using Xunit;

namespace AuditDataPlatform.Domain.Tests;

public class CrossFieldConsistencyEvaluatorTests
{
    [Fact]
    public void Evaluate_ConditionHolds_ReturnsNoFindings()
    {
        var rule = TestRuleFactory.CreateRule(RuleType.CrossFieldConsistency,
            """{"left":"EndDate","operator":">=","right":"StartDate"}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["StartDate"] = "2026-01-01", ["EndDate"] = "2026-01-10" }));

        var results = new CrossFieldConsistencyEvaluator().Evaluate(context).ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void Evaluate_ConditionViolated_ReturnsFinding()
    {
        var rule = TestRuleFactory.CreateRule(RuleType.CrossFieldConsistency,
            """{"left":"EndDate","operator":">=","right":"StartDate"}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["StartDate"] = "2026-01-10", ["EndDate"] = "2026-01-01" }));

        var results = new CrossFieldConsistencyEvaluator().Evaluate(context).ToList();

        Assert.Single(results);
    }

    [Fact]
    public void Evaluate_ValuesNotComparable_IsSkipped()
    {
        var rule = TestRuleFactory.CreateRule(RuleType.CrossFieldConsistency,
            """{"left":"EndDate","operator":">=","right":"StartDate"}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["StartDate"] = "abc", ["EndDate"] = "def" }));

        var results = new CrossFieldConsistencyEvaluator().Evaluate(context).ToList();

        Assert.Empty(results);
    }
}