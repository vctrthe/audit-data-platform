using System.Text.Json;

namespace AuditDataPlatform.Domain.RuleEngine.Evaluators;

internal static class RuleParameterJson
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
    };
}
