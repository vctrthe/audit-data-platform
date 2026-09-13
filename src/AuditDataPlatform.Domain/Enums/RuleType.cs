namespace AuditDataPlatform.Domain.Enums;

public enum RuleType
{
    RequiredField,
    DataFormat,
    Duplicate,
    RangeThreshold,
    CrossFieldConsistency,
    PatternMatch,
    StatisticalOutlier,
}
