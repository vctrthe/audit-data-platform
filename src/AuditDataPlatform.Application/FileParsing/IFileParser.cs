using AuditDataPlatform.Application.Common;

namespace AuditDataPlatform.Application.FileParsing;

public interface IFileParser
{
    string SupportedFormat { get; } // "csv" | "xlsx" — must match ImportJob.SourceFormat
    Result<ParsedFile> Parse(Stream fileStream);
}