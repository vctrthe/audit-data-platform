namespace AuditDataPlatform.Application.FileParsing;

public sealed class ParsedFile
{
    public required IReadOnlyList<string> Columns { get; init; }
    public required IReadOnlyList<ParsedRow> Rows { get; init; }
}

public sealed class ParsedRow
{
    public required int RowNumber { get; init; }
    public required IReadOnlyDictionary<string, string?> Values { get; init; }
}