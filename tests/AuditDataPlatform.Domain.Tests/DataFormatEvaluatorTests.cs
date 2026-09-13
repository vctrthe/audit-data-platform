using AuditDataPlatform.Domain.Enums;
using AuditDataPlatform.Domain.RuleEngine.Evaluators;
using Xunit;

namespace AuditDataPlatform.Domain.Tests;

public class DataFormatEvaluatorTests
{
    [Fact]
    public void Evaluate_ValueMatchesFormat_ReturnsNoFindings()
    {
        var rule = TestRuleFactory.CreateRule(RuleType.DataFormat, """{"column":"Date","format":"yyyy-MM-dd"}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["Date"] = "2026-09-13" }));

        var results = new DataFormatEvaluator().Evaluate(context).ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void Evaluate_ValueDoesNotMatchFormat_ReturnsFinding()
    {
        var rule = TestRuleFactory.CreateRule(RuleType.DataFormat, """{"column":"Date","format":"yyyy-MM-dd"}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["Date"] = "13/09/2026" }));

        var results = new DataFormatEvaluator().Evaluate(context).ToList();

        var result = Assert.Single(results);
        Assert.Equal(1, result.RowNumber);
    }

    [Fact]
    public void Evaluate_ValueMissing_IsSkipped_NotThisEvaluatorsConcern()
    {
        var rule = TestRuleFactory.CreateRule(RuleType.DataFormat, """{"column":"Date","format":"yyyy-MM-dd"}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["Date"] = "" }));

        var results = new DataFormatEvaluator().Evaluate(context).ToList();

        Assert.Empty(results);
    }
}