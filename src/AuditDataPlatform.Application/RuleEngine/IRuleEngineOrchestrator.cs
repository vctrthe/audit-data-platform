using AuditDataPlatform.Domain.Entities;

namespace AuditDataPlatform.Application.RuleEngine;

public interface IRuleEngineOrchestrator
{
    IReadOnlyList<Finding> Evaluate(RuleSet ruleSet, IReadOnlyList<ImportRow> importRows);
}