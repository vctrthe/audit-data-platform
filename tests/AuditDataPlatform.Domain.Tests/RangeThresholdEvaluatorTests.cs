using AuditDataPlatform.Domain.Enums;
using AuditDataPlatform.Domain.RuleEngine.Evaluators;
using Xunit;

namespace AuditDataPlatform.Domain.Tests;

public class RangeThresholdEvaluatorTests
{
    [Fact]
    public void Evaluate_ValueWithinRange_ReturnsNoFindings()
    {
        var rule = TestRuleFactory.CreateRule(RuleType.RangeThreshold, """{"column":"Amount","min":0,"max":1000000}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["Amount"] = "500" }));

        var results = new RangeThresholdEvaluator().Evaluate(context).ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void Evaluate_ValueBelowMin_ReturnsFinding()
    {
        var rule = TestRuleFactory.CreateRule(RuleType.RangeThreshold, """{"column":"Amount","min":0,"max":1000000}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["Amount"] = "-5" }));

        var results = new RangeThresholdEvaluator().Evaluate(context).ToList();

        Assert.Single(results);
    }

    [Fact]
    public void Evaluate_NonNumericValue_IsSkipped_NotThisEvaluatorsConcern()
    {
        var rule = TestRuleFactory.CreateRule(RuleType.RangeThreshold, """{"column":"Amount","min":0,"max":1000000}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["Amount"] = "not-a-number" }));

        var results = new RangeThresholdEvaluator().Evaluate(context).ToList();

        Assert.Empty(results);
    }
}