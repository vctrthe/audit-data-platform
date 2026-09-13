using AuditDataPlatform.Application.Common;
using AuditDataPlatform.Application.FileParsing;
using ClosedXML.Excel;

namespace AuditDataPlatform.Infrastructure.FileParsing;

public sealed class XlsxFileParser : IFileParser
{
    public string SupportedFormat => "xlsx";

    public Result<ParsedFile> Parse(Stream fileStream)
    {
        try
        {
            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet is null)
            {
                return Result<ParsedFile>.Failure("Workbook has no worksheets.");
            }

            var usedRange = worksheet.RangeUsed();
            if (usedRange is null)
            {
                return Result<ParsedFile>.Failure("Worksheet is empty.");
            }

            var headerRow = usedRange.FirstRow();
            var columns = headerRow.Cells()
                .Select(c => c.GetString().Trim())
                .ToList();

            if (columns.Count == 0 || columns.Any(string.IsNullOrWhiteSpace))
            {
                return Result<ParsedFile>.Failure("Header row has empty column names.");
            }

            var rows = new List<ParsedRow>();
            var rowNumber = 0;

            foreach (var dataRow in usedRange.RowsUsed().Skip(1))
            {
                rowNumber++;
                var values = new Dictionary<string, string?>();
                for (var i = 0; i < columns.Count; i++)
                {
                    var cell = dataRow.Cell(i + 1);
                    values[columns[i]] = cell.IsEmpty() ? null : cell.GetString();
                }

                rows.Add(new ParsedRow { RowNumber = rowNumber, Values = values });
            }

            return Result<ParsedFile>.Success(new ParsedFile { Columns = columns, Rows = rows });
        }
        catch (Exception ex)
        {
            return Result<ParsedFile>.Failure($"Failed to parse XLSX file: {ex.Message}");
        }
    }
}