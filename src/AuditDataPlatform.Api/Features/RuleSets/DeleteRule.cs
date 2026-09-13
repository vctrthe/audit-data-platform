using AuditDataPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuditDataPlatform.Api.Features.RuleSets;

public static class DeleteRule
{
    public static void MapDeleteRule(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/rulesets/{ruleSetId:guid}/rules/{ruleId:guid}", Handle);
    }

    private static async Task<IResult> Handle(Guid ruleSetId, Guid ruleId, AppDbContext db, CancellationToken ct)
    {
        var rule = await db.Rules.FirstOrDefaultAsync(r => r.Id == ruleId && r.RuleSetId == ruleSetId, ct);
        if (rule is null)
        {
            return Results.NotFound(new { error = $"Rule '{ruleId}' not found in RuleSet '{ruleSetId}'." });
        }

        try
        {
            db.Rules.Remove(rule);
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            // Finding→Rule FK is Restrict by design (audit trail) — a rule that already
            // produced findings can't be hard-deleted.
            return Results.Conflict(new
            {
                error = $"Rule '{ruleId}' cannot be deleted because it has produced findings. Set IsActive to false instead."
            });
        }

        return Results.NoContent();
    }
}