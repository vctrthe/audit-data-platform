using System.Globalization;
using AuditDataPlatform.Application.Common;
using AuditDataPlatform.Application.FileParsing;
using CsvHelper;
using CsvHelper.Configuration;

namespace AuditDataPlatform.Infrastructure.FileParsing;

public sealed class CsvFileParser : IFileParser
{
    public string SupportedFormat => "csv";

    public Result<ParsedFile> Parse(Stream fileStream)
    {
        try
        {
            using var reader = new StreamReader(fileStream);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null
            };
            using var csv = new CsvReader(reader, config);

            if (!csv.Read() || !csv.ReadHeader())
            {
                return Result<ParsedFile>.Failure("CSV file is empty or has no header row.");
            }

            var columns = csv.HeaderRecord ?? Array.Empty<string>();
            if (columns.Length == 0)
            {
                return Result<ParsedFile>.Failure("CSV header row has no columns.");
            }

            var rows = new List<ParsedRow>();
            var rowNumber = 0;

            while (csv.Read())
            {
                rowNumber++;
                var values = new Dictionary<string, string?>();
                foreach (var column in columns)
                {
                    values[column] = csv.GetField(column);
                }

                rows.Add(new ParsedRow { RowNumber = rowNumber, Values = values });
            }

            return Result<ParsedFile>.Success(new ParsedFile { Columns = columns, Rows = rows });
        }
        catch (Exception ex)
        {
            return Result<ParsedFile>.Failure($"Failed to parse CSV file: {ex.Message}");
        }
    }
}