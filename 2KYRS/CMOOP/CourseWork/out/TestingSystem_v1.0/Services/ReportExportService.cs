using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Security;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Services;

/// <summary>
/// Экспорт: CSV (UTF-8 с BOM, разделитель «;» — открывается в Excel), XLSX (минимальный OpenXML без внешних
/// библиотек) и PDF (QuestPDF) — отчёты и именные сертификаты. Подписи колонок локализуются текущим языком.
/// </summary>
public class ReportExportService : IReportExportService
{
    private readonly ILocalizationService _loc;

    static ReportExportService() => QuestPDF.Settings.License = LicenseType.Community;

    public ReportExportService(ILocalizationService loc) => _loc = loc;

    private string[] Headers() => new[]
    {
        "Col_Student", "Col_Group", "Col_Test", "Col_Points", "Col_Percent", "Col_Passed", "Col_Status", "Col_Date", "Col_Violations"
    }.Select(k => _loc[k]).ToArray();

    private string Yes(bool v) => _loc[v ? "Yes" : "No"];
    private string StatusText(ResultStatus s) => _loc[$"Status_{s}"];

    // ============================== CSV ==============================

    public void ExportResultsCsv(IReadOnlyList<ResultExportRow> rows, string path)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(";", Headers().Select(Escape)));
        foreach (var r in rows)
        {
            sb.AppendLine(string.Join(";", new[]
            {
                r.Student, r.Group, r.Test,
                $"{r.Score.ToString("0.##", CultureInfo.InvariantCulture)}/{r.MaxScore.ToString("0.##", CultureInfo.InvariantCulture)}",
                r.Percent.ToString("0.0", CultureInfo.InvariantCulture), Yes(r.Passed), StatusText(r.Status),
                r.Date.ToString("yyyy-MM-dd HH:mm"), r.Violations.ToString()
            }.Select(Escape)));
        }
        File.WriteAllText(path, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
    }

    private static string Escape(string value)
        => value.IndexOfAny(new[] { ';', '"', '\n', '\r' }) >= 0 ? $"\"{value.Replace("\"", "\"\"")}\"" : value;

    // ============================== XLSX ==============================

    public void ExportResultsExcel(IReadOnlyList<ResultExportRow> rows, string path)
    {
        var table = new List<object[]> { Headers().Cast<object>().ToArray() };
        foreach (var r in rows)
            table.Add(new object[]
            {
                r.Student, r.Group, r.Test, $"{r.Score:0.##}/{r.MaxScore:0.##}", r.Percent, Yes(r.Passed),
                StatusText(r.Status), r.Date.ToString("yyyy-MM-dd HH:mm"), (double)r.Violations
            });
        WriteXlsx(path, table);
    }

    /// <summary>Минимальный валидный .xlsx: zip из нескольких XML-файлов; строки — inlineStr, числа — как есть.</summary>
    private static void WriteXlsx(string path, List<object[]> table)
    {
        if (File.Exists(path)) File.Delete(path);
        using var zip = ZipFile.Open(path, ZipArchiveMode.Create);

        Add(zip, "[Content_Types].xml",
            "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">" +
            "<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/><Default Extension=\"xml\" ContentType=\"application/xml\"/>" +
            "<Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/>" +
            "<Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/></Types>");

        Add(zip, "_rels/.rels",
            "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
            "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/></Relationships>");

        Add(zip, "xl/workbook.xml",
            "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" " +
            "xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"><sheets><sheet name=\"Results\" sheetId=\"1\" r:id=\"rId1\"/></sheets></workbook>");

        Add(zip, "xl/_rels/workbook.xml.rels",
            "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
            "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/></Relationships>");

        var sheet = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><sheetData>");
        for (int r = 0; r < table.Count; r++)
        {
            sheet.Append($"<row r=\"{r + 1}\">");
            for (int c = 0; c < table[r].Length; c++)
            {
                string cell = $"{(char)('A' + c)}{r + 1}";
                if (table[r][c] is double d)
                    sheet.Append($"<c r=\"{cell}\"><v>{d.ToString(CultureInfo.InvariantCulture)}</v></c>");
                else
                    sheet.Append($"<c r=\"{cell}\" t=\"inlineStr\"><is><t>{SecurityElement.Escape(table[r][c]?.ToString() ?? "")}</t></is></c>");
            }
            sheet.Append("</row>");
        }
        sheet.Append("</sheetData></worksheet>");
        Add(zip, "xl/worksheets/sheet1.xml", sheet.ToString());

        static void Add(ZipArchive z, string name, string content)
        {
            var entry = z.CreateEntry(name);
            using var w = new StreamWriter(entry.Open(), new UTF8Encoding(false));
            w.Write(content);
        }
    }

    // ============================== PDF: отчёт ==============================

    public void ExportReportPdf(ReportData report, string path)
    {
        var headers = new[] { "Col_Student", "Col_Group", "Col_Test", "Col_Points", "Col_Percent", "Col_Passed", "Col_Date" }
            .Select(k => _loc[k]).ToArray();

        Document.Create(doc => doc.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(30);
            page.DefaultTextStyle(t => t.FontFamily("Segoe UI").FontSize(10));

            page.Header().Column(col =>
            {
                col.Item().Text(report.Title).FontSize(20).Bold();
                col.Item().Text(report.FilterDescription).FontColor(Colors.Grey.Darken1);
                col.Item().Text($"{_loc["Report_Generated"]}: {DateTime.Now:dd.MM.yyyy HH:mm}").FontColor(Colors.Grey.Darken1);
            });

            page.Content().PaddingVertical(10).Column(col =>
            {
                col.Spacing(12);

                col.Item().Table(t =>
                {
                    t.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(3); c.RelativeColumn(1.6f); c.RelativeColumn(3);
                        c.RelativeColumn(1.4f); c.RelativeColumn(1.2f); c.RelativeColumn(1.2f); c.RelativeColumn(2);
                    });
                    t.Header(h =>
                    {
                        foreach (var text in headers)
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text(text).Bold();
                    });
                    foreach (var r in report.Rows)
                    {
                        t.Cell().Padding(3).Text(r.Student);
                        t.Cell().Padding(3).Text(r.Group);
                        t.Cell().Padding(3).Text(r.Test);
                        t.Cell().Padding(3).Text($"{r.Score:0.##}/{r.MaxScore:0.##}");
                        t.Cell().Padding(3).Text($"{r.Percent:0.#}%");
                        t.Cell().Padding(3).Text(Yes(r.Passed));
                        t.Cell().Padding(3).Text(r.Date.ToString("dd.MM.yyyy HH:mm"));
                    }
                });

                if (report.HardQuestions.Count > 0)
                {
                    col.Item().Text(_loc["Report_HardQuestions"]).FontSize(14).Bold();
                    foreach (var q in report.HardQuestions.Take(15))
                        col.Item().Text($"• [{q.TestTitle}] {q.QuestionText} — {q.WrongRatePercent:0.#}% ({q.Attempts})")
                            .FontColor(q.IsHard ? Colors.Red.Darken2 : Colors.Black);
                }
            });

            page.Footer().AlignCenter().Text(x =>
            {
                x.CurrentPageNumber();
                x.Span(" / ");
                x.TotalPages();
            });
        })).GeneratePdf(path);
    }

    // ============================== PDF: сертификат ==============================

    public void ExportCertificatePdf(CertificateData c, string path)
    {
        Document.Create(doc => doc.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(30);
            page.DefaultTextStyle(t => t.FontFamily("Segoe UI").FontSize(16));

            page.Content()
                .Border(4).BorderColor(Colors.Blue.Darken2)
                .Padding(40)
                .Column(col =>
                {
                    col.Spacing(18);
                    col.Item().AlignCenter().Text(_loc["Cert_Title"]).FontSize(42).Bold().FontColor(Colors.Blue.Darken2);
                    col.Item().AlignCenter().Text(_loc["Cert_Awarded"]).FontSize(18);
                    col.Item().AlignCenter().Text(c.StudentName).FontSize(32).Bold();
                    col.Item().AlignCenter().Text(_loc["Cert_ForPassing"]).FontSize(18);
                    col.Item().AlignCenter().Text($"«{c.TestTitle}»").FontSize(26).SemiBold();
                    col.Item().AlignCenter().Text($"{_loc["Cert_Result"]}: {c.Percent:0.#}%").FontSize(20);
                    col.Item().PaddingTop(30).Row(row =>
                    {
                        row.RelativeItem().Text($"{_loc["Cert_Date"]}: {c.Date:dd.MM.yyyy}");
                        row.RelativeItem().AlignRight().Text($"№ {c.Number}");
                    });
                });
        })).GeneratePdf(path);
    }
}
