using AuditDataPlatform.Domain.Enums;

namespace AuditDataPlatform.Domain.RuleEngine;

public interface IRuleEvaluator
{
    RuleType SupportedType { get; }
    IEnumerable<RuleEvaluationResult> Evaluate(RuleEvaluationContext context);
}