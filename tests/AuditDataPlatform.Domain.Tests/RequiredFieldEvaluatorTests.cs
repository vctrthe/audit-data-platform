using AuditDataPlatform.Domain.Enums;
using AuditDataPlatform.Domain.RuleEngine.Evaluators;
using Xunit;

namespace AuditDataPlatform.Domain.Tests;

public class RequiredFieldEvaluatorTests
{
    [Fact]
    public void Evaluate_ValuePresent_ReturnsNoFindings()
    {
        var rule = TestRuleFactory.CreateRule(RuleType.RequiredField, """{"column":"InvoiceNo"}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["InvoiceNo"] = "INV-001" }));

        var results = new RequiredFieldEvaluator().Evaluate(context).ToList();

        Assert.Empty(results);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Evaluate_ValueMissingOrBlank_ReturnsFinding(string? value)
    {
        var rule = TestRuleFactory.CreateRule(RuleType.RequiredField, """{"column":"InvoiceNo"}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["InvoiceNo"] = value }));

        var results = new RequiredFieldEvaluator().Evaluate(context).ToList();

        var result = Assert.Single(results);
        Assert.Equal(1, result.RowNumber);
        Assert.Equal("InvoiceNo", result.Column);
    }

    [Fact]
    public void Evaluate_ColumnEntirelyMissingFromRow_ReturnsFinding()
    {
        var rule = TestRuleFactory.CreateRule(RuleType.RequiredField, """{"column":"InvoiceNo"}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["OtherColumn"] = "x" }));

        var results = new RequiredFieldEvaluator().Evaluate(context).ToList();

        Assert.Single(results);
    }
}