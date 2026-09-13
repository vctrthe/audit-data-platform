using AuditDataPlatform.Domain.Enums;
using AuditDataPlatform.Domain.RuleEngine.Evaluators;
using Xunit;

namespace AuditDataPlatform.Domain.Tests;

public class PatternMatchEvaluatorTests
{
    [Fact]
    public void Evaluate_MustMatch_ValueMatches_ReturnsNoFindings()
    {
        var rule = TestRuleFactory.CreateRule(RuleType.PatternMatch,
            """{"column":"Notes","regex":"^[A-Za-z0-9 ]+$","mode":"mustMatch"}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["Notes"] = "Reviewed OK" }));

        var results = new PatternMatchEvaluator().Evaluate(context).ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void Evaluate_MustMatch_ValueDoesNotMatch_ReturnsFinding()
    {
        var rule = TestRuleFactory.CreateRule(RuleType.PatternMatch,
            """{"column":"Notes","regex":"^[A-Za-z0-9 ]+$","mode":"mustMatch"}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["Notes"] = "Bad$Char!" }));

        var results = new PatternMatchEvaluator().Evaluate(context).ToList();

        Assert.Single(results);
    }

    [Fact]
    public void Evaluate_MustNotMatch_ValueMatchesForbiddenPattern_ReturnsFinding()
    {
        var rule = TestRuleFactory.CreateRule(RuleType.PatternMatch,
            """{"column":"Notes","regex":"CONFIDENTIAL","mode":"mustNotMatch"}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["Notes"] = "marked CONFIDENTIAL" }));

        var results = new PatternMatchEvaluator().Evaluate(context).ToList();

        Assert.Single(results);
    }
}