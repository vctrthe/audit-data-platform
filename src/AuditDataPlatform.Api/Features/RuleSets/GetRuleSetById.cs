using AuditDataPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuditDataPlatform.Api.Features.RuleSets;

public static class GetRuleSetById
{
    public sealed record RuleResponse(Guid Id, string Name, string Type, string Severity, bool IsActive, string ParametersJson, int SortOrder);
    public sealed record Response(Guid Id, string Name, string? Description, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, IReadOnlyList<RuleResponse> Rules);

    public static void MapGetRuleSetById(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/rulesets/{id:guid}", Handle);
    }

    private static async Task<IResult> Handle(Guid id, AppDbContext db, CancellationToken ct)
    {
        var ruleSet = await db.RuleSets
            .Include(r => r.Rules)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        if (ruleSet is null)
        {
            return Results.NotFound(new { error = $"RuleSet '{id}' not found." });
        }

        var response = new Response(
            ruleSet.Id,
            ruleSet.Name,
            ruleSet.Description,
            ruleSet.CreatedAt,
            ruleSet.UpdatedAt,
            ruleSet.Rules
                .OrderBy(r => r.SortOrder)
                .Select(r => new RuleResponse(r.Id, r.Name, r.Type.ToString(), r.Severity.ToString(), r.IsActive, r.ParametersJson, r.SortOrder))
                .ToList());

        return Results.Ok(response);
    }
}