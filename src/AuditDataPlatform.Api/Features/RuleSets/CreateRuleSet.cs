using AuditDataPlatform.Domain.Entities;
using AuditDataPlatform.Infrastructure.Persistence;

namespace AuditDataPlatform.Api.Features.RuleSets;

public static class CreateRuleSet
{
    public sealed record Request(string Name, string? Description);
    public sealed record Response(Guid Id, string Name, string? Description, DateTimeOffset CreatedAt);

    public static void MapCreateRuleSet(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/rulesets", Handle);
    }

    private static async Task<IResult> Handle(Request request, AppDbContext db, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Results.BadRequest(new { error = "Name is required." });
        }

        if (request.Name.Length > 200)
        {
            return Results.BadRequest(new { error = "Name must be 200 characters or fewer." });
        }

        var ruleSet = new RuleSet
        {
            Name = request.Name.Trim(),
            Description = request.Description
        };

        db.RuleSets.Add(ruleSet);
        await db.SaveChangesAsync(ct);

        return Results.Created(
            $"/api/rulesets/{ruleSet.Id}",
            new Response(ruleSet.Id, ruleSet.Name, ruleSet.Description, ruleSet.CreatedAt));
    }
}