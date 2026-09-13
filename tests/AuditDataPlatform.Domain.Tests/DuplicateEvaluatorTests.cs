using AuditDataPlatform.Domain.Enums;
using AuditDataPlatform.Domain.RuleEngine.Evaluators;
using Xunit;

namespace AuditDataPlatform.Domain.Tests;

public class DuplicateEvaluatorTests
{
    [Fact]
    public void Evaluate_AllRowsUnique_ReturnsNoFindings()
    {
        var rule = TestRuleFactory.CreateRule(RuleType.Duplicate, """{"keyColumns":["InvoiceNo"]}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["InvoiceNo"] = "INV-001" }),
            (2, new Dictionary<string, string?> { ["InvoiceNo"] = "INV-002" }));

        var results = new DuplicateEvaluator().Evaluate(context).ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void Evaluate_TwoRowsShareKey_BothFlagged()
    {
        var rule = TestRuleFactory.CreateRule(RuleType.Duplicate, """{"keyColumns":["InvoiceNo","Date"]}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["InvoiceNo"] = "INV-001", ["Date"] = "2026-09-13" }),
            (2, new Dictionary<string, string?> { ["InvoiceNo"] = "INV-001", ["Date"] = "2026-09-13" }),
            (3, new Dictionary<string, string?> { ["InvoiceNo"] = "INV-003", ["Date"] = "2026-09-14" }));

        var results = new DuplicateEvaluator().Evaluate(context).ToList();

        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.RowNumber == 1);
        Assert.Contains(results, r => r.RowNumber == 2);
    }

    [Fact]
    public void Evaluate_RowsWithBlankKeyColumns_AreNotFalselyGroupedAsDuplicates()
    {
        var rule = TestRuleFactory.CreateRule(RuleType.Duplicate, """{"keyColumns":["InvoiceNo"]}""");
        var context = TestRuleFactory.CreateContext(rule,
            (1, new Dictionary<string, string?> { ["InvoiceNo"] = "" }),
            (2, new Dictionary<string, string?> { ["InvoiceNo"] = "" }));

        var results = new DuplicateEvaluator().Evaluate(context).ToList();

        Assert.Empty(results);
    }
}