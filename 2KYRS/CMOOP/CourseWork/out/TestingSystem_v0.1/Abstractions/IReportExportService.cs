using TestingSystem.Models;

namespace TestingSystem.Abstractions;

/// <summary>Экспорт данных. Реализация (CSV, XLSX, PDF) скрыта за интерфейсом и заменяема.</summary>
public interface IReportExportService
{
    void ExportResultsCsv(IReadOnlyList<ResultExportRow> rows, string path);
    void ExportResultsExcel(IReadOnlyList<ResultExportRow> rows, string path);
    void ExportReportPdf(ReportData report, string path);
    void ExportCertificatePdf(CertificateData certificate, string path);
}
