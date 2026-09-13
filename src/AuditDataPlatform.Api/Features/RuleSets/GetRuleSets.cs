using AuditDataPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuditDataPlatform.Api.Features.RuleSets;

public static class GetRuleSets
{
    public sealed record Response(Guid Id, string Name, string? Description, int RuleCount, DateTimeOffset CreatedAt);

    public static void MapGetRuleSets(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/rulesets", Handle);
    }

    private static async Task<IResult> Handle(AppDbContext db, CancellationToken ct)
    {
        var ruleSets = await db.RuleSets
            .OrderBy(r => r.Name)
            .Select(r => new Response(r.Id, r.Name, r.Description, r.Rules.Count, r.CreatedAt))
            .ToListAsync(ct);

        return Results.Ok(ruleSets);
    }
}