using System.Text.Json;
using AuditDataPlatform.Domain.Entities;
using AuditDataPlatform.Domain.Enums;
using AuditDataPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuditDataPlatform.Api.Features.RuleSets;

public static class AddRule
{
    public sealed record Request(string Name, RuleType Type, Severity Severity, string ParametersJson, int SortOrder, bool IsActive);
    public sealed record Response(Guid Id, string Name, string Type, string Severity, string ParametersJson, int SortOrder, bool IsActive);

    public static void MapAddRule(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/rulesets/{ruleSetId:guid}/rules", Handle);
    }

    private static async Task<IResult> Handle(Guid ruleSetId, Request request, AppDbContext db, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Results.BadRequest(new { error = "Name is required." });
        }

        if (!IsValidJson(request.ParametersJson))
        {
            return Results.BadRequest(new { error = "ParametersJson is not valid JSON." });
        }

        var ruleSetExists = await db.RuleSets.AnyAsync(r => r.Id == ruleSetId, ct);
        if (!ruleSetExists)
        {
            return Results.NotFound(new { error = $"RuleSet '{ruleSetId}' not found." });
        }

        var rule = new Rule
        {
            RuleSetId = ruleSetId,
            Name = request.Name.Trim(),
            Type = request.Type,
            Severity = request.Severity,
            ParametersJson = request.ParametersJson,
            SortOrder = request.SortOrder,
            IsActive = request.IsActive
        };

        db.Rules.Add(rule);
        await db.SaveChangesAsync(ct);

        return Results.Created(
            $"/api/rulesets/{ruleSetId}/rules/{rule.Id}",
            new Response(rule.Id, rule.Name, rule.Type.ToString(), rule.Severity.ToString(), rule.ParametersJson, rule.SortOrder, rule.IsActive));
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