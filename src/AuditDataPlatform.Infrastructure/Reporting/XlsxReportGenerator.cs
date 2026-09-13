using AuditDataPlatform.Application.Reporting;
using AuditDataPlatform.Domain.Entities;
using AuditDataPlatform.Domain.Enums;
using ClosedXML.Excel;

namespace AuditDataPlatform.Infrastructure.Reporting;

public sealed class XlsxReportGenerator : IXlsxReportGenerator
{
    public byte[] Generate(ImportJob importJob, IReadOnlyList<Finding> findings)
    {
        using var workbook = new XLWorkbook();

        WriteSummarySheet(workbook, importJob, findings);
        WriteFindingsSheet(workbook, findings);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static void WriteSummarySheet(XLWorkbook workbook, ImportJob importJob, IReadOnlyList<Finding> findings)
    {
        var sheet = workbook.Worksheets.Add("Summary");
        var row = 1;

        void WriteRow(string label, Action writeValue)
        {
            sheet.Cell(row, 1).Value = label;
            writeValue();
            row++;
        }

        WriteRow("File name", () => sheet.Cell(row, 2).Value = importJob.OriginalFileName);
        WriteRow("Source format", () => sheet.Cell(row, 2).Value = importJob.SourceFormat);
        WriteRow("Status", () => sheet.Cell(row, 2).Value = importJob.Status.ToString());
        WriteRow("Uploaded at (UTC)", () => sheet.Cell(row, 2).Value = importJob.UploadedAt.UtcDateTime);
        WriteRow("Completed at (UTC)", () => sheet.Cell(row, 2).Value = importJob.CompletedAt?.UtcDateTime.ToString("u") ?? "-");
        WriteRow("Total rows", () => sheet.Cell(row, 2).Value = importJob.TotalRows);
        WriteRow("Error findings", () => sheet.Cell(row, 2).Value = findings.Count(f => f.Severity == Severity.Error));
        WriteRow("Warning findings", () => sheet.Cell(row, 2).Value = findings.Count(f => f.Severity == Severity.Warning));
        WriteRow("Info findings", () => sheet.Cell(row, 2).Value = findings.Count(f => f.Severity == Severity.Info));

        sheet.Column(1).Style.Font.Bold = true;
        sheet.Columns().AdjustToContents();
    }

    private static void WriteFindingsSheet(XLWorkbook workbook, IReadOnlyList<Finding> findings)
    {
        var sheet = workbook.Worksheets.Add("Findings");

        var headers = new[] { "Row", "Column", "Rule", "Severity", "Message" };
        for (var i = 0; i < headers.Length; i++)
        {
            sheet.Cell(1, i + 1).Value = headers[i];
        }
        sheet.Row(1).Style.Font.Bold = true;

        var rowIndex = 2;
        foreach (var finding in findings.OrderBy(f => f.RowNumber))
        {
            sheet.Cell(rowIndex, 1).Value = finding.RowNumber;
            sheet.Cell(rowIndex, 2).Value = finding.Column ?? "-";
            sheet.Cell(rowIndex, 3).Value = finding.Rule?.Name ?? "(unknown rule)";
            sheet.Cell(rowIndex, 4).Value = finding.Severity.ToString();
            sheet.Cell(rowIndex, 5).Value = finding.Message;
            rowIndex++;
        }

        sheet.Columns().AdjustToContents();
    }
}