using AuditDataPlatform.Domain.Entities;

namespace AuditDataPlatform.Application.Reporting;

public interface IXlsxReportGenerator
{
    byte[] Generate(ImportJob importJob, IReadOnlyList<Finding> findings);
}