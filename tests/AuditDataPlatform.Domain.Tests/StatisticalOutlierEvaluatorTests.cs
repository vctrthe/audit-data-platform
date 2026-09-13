using AuditDataPlatform.Domain.Enums;
using AuditDataPlatform.Domain.RuleEngine.Evaluators;
using Xunit;

namespace AuditDataPlatform.Domain.Tests;

public class StatisticalOutlierEvaluatorTests
{
    [Fact]
    public void Evaluate_NoOutliers_ReturnsNoFindings()
    {
        var rule = TestRuleFactory.CreateRule(RuleType.StatisticalOutlier, """{"column":"Amount","zScoreThreshold":3}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["Amount"] = "100" }),
            (2, new Dictionary<string, string?> { ["Amount"] = "105" }),
            (3, new Dictionary<string, string?> { ["Amount"] = "98" }));

        var results = new StatisticalOutlierEvaluator().Evaluate(context).ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void Evaluate_OneExtremeValue_IsFlaggedAsOutlier()
    {
        var rule = TestRuleFactory.CreateRule(RuleType.StatisticalOutlier, """{"column":"Amount","zScoreThreshold":1.5}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["Amount"] = "10" }),
            (2, new Dictionary<string, string?> { ["Amount"] = "10" }),
            (3, new Dictionary<string, string?> { ["Amount"] = "10" }),
            (4, new Dictionary<string, string?> { ["Amount"] = "10" }),
            (5, new Dictionary<string, string?> { ["Amount"] = "10" }),
            (6, new Dictionary<string, string?> { ["Amount"] = "1000" }));

        var results = new StatisticalOutlierEvaluator().Evaluate(context).ToList();

        var result = Assert.Single(results);
        Assert.Equal(6, result.RowNumber);
    }

    [Fact]
    public void Evaluate_FewerThanTwoNumericRows_ReturnsNoFindings()
    {
        var rule = TestRuleFactory.CreateRule(RuleType.StatisticalOutlier, """{"column":"Amount","zScoreThreshold":3}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["Amount"] = "100" }));

        var results = new StatisticalOutlierEvaluator().Evaluate(context).ToList();

        Assert.Empty(results);
    }
}