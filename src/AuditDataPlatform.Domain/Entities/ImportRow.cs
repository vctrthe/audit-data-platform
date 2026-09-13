namespace AuditDataPlatform.Domain.Entities;

public class ImportRow
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required Guid ImportJobId { get; init; }
    public ImportJob? ImportJob { get; init; }

    public required int RowNumber { get; set; }
    public required string DataJson { get; set; }
    public bool HasFindings { get; set; }

    public ICollection<Finding> Findings { get; init; } = new List<Finding>();
}
