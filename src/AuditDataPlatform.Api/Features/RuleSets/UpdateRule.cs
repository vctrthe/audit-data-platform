using System.Text.Json;
using AuditDataPlatform.Domain.Enums;
using AuditDataPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuditDataPlatform.Api.Features.RuleSets;

public static class UpdateRule
{
    public sealed record Request(string Name, RuleType Type, Severity Severity, string ParametersJson, int SortOrder, bool IsActive);
    public sealed record Response(Guid Id, string Name, string Type, string Severity, string ParametersJson, int SortOrder, bool IsActive);

    public static void MapUpdateRule(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/rulesets/{ruleSetId:guid}/rules/{ruleId:guid}", Handle);
    }

    private static async Task<IResult> Handle(Guid ruleSetId, Guid ruleId, Request request, AppDbContext db, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Results.BadRequest(new { error = "Name is required." });
        }

        if (!IsValidJson(request.ParametersJson))
        {
            return Results.BadRequest(new { error = "ParametersJson is not valid JSON." });
        }

        var rule = await db.Rules.FirstOrDefaultAsync(r => r.Id == ruleId && r.RuleSetId == ruleSetId, ct);
        if (rule is null)
        {
            return Results.NotFound(new { error = $"Rule '{ruleId}' not found in RuleSet '{ruleSetId}'." });
        }

        rule.Name = request.Name.Trim();
        rule.Type = request.Type;
        rule.Severity = request.Severity;
        rule.ParametersJson = request.ParametersJson;
        rule.SortOrder = request.SortOrder;
        rule.IsActive = request.IsActive;

        await db.SaveChangesAsync(ct);

        return Results.Ok(new Response(rule.Id, rule.Name, rule.Type.ToString(), rule.Severity.ToString(), rule.ParametersJson, rule.SortOrder, rule.IsActive));
    }

    private static bool IsValidJson(string json)
    {
        try
        {
            using var _ = JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}