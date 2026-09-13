namespace AuditDataPlatform.Domain.Entities;

public class RuleSet
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Rule> Rules { get; init; } = new List<Rule>();
    public ICollection<ImportJob> ImportJobs { get; init; } = new List<ImportJob>();
}
