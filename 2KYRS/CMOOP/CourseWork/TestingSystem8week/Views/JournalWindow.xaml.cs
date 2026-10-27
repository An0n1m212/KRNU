using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Views;

/// <summary>Тиждень 6 (ТЗ п.3.8): журнал з фільтрами, складність питань, експорт Excel/CSV/PDF.</summary>
public partial class JournalWindow : Window
{
    private readonly ServiceProvider _sp;

    public JournalWindow(ServiceProvider sp)
    {
        InitializeComponent();
        _sp = sp;
        var groups = _sp.GetRequiredService<IGroupRepository>().GetAll().ToList();
        groups.Insert(0, new Group { Name = "Усі" });
        GroupBox.ItemsSource = groups;
        GroupBox.SelectedIndex = 0;
        var tests = _sp.GetRequiredService<ITestRepository>().GetAll().ToList();
        tests.Insert(0, new Test { Title = "Усі" });
        TestBox.ItemsSource = tests;
        TestBox.SelectedIndex = 0;
        Refresh();
    }

    private List<ResultExportRow> CurrentRows()
    {
        var results = _sp.GetRequiredService<IResultRepository>().GetAll().AsEnumerable();
        var users = _sp.GetRequiredService<IUserRepository>();
        var groups = _sp.GetRequiredService<IGroupRepository>().GetAll().ToDictionary(g => g.Id, g => g.Name);
        var tests = _sp.GetRequiredService<ITestRepository>().GetAll().ToDictionary(t => t.Id, t => t.Title);
        if (GroupBox.SelectedItem is Group g && g.Id != 0)
            results = results.Where(r => users.GetById(r.StudentId)?.GroupId == g.Id);
        if (TestBox.SelectedItem is Test t && t.Id != 0)
            results = results.Where(r => r.TestId == t.Id);
        return results.OrderByDescending(r => r.Date).Select(r =>
        {
            var u = users.GetById(r.StudentId);
            return new ResultExportRow(
                u?.FullName ?? "?", u?.GroupId is int gid && groups.TryGetValue(gid, out var gn) ? gn : "—",
                tests.TryGetValue(r.TestId, out var tn) ? tn : "—",
                r.TotalScore, r.MaxScore, r.Percentage, r.IsPassed, r.Status, r.Date, 0);
        }).ToList();
    }

    private void Refresh()
    {
        var rows = CurrentRows();
        RowsList.ItemsSource = rows.Select(r => $"{r.Date:yyyy-MM-dd HH:mm} — {r.Student} ({r.Group}) — {r.Test}: {r.Score}/{r.MaxScore} ({r.Percent}%) — {r.Status}").ToList();
        var all = _sp.GetRequiredService<IResultRepository>().GetAll();
        var hard = _sp.GetRequiredService<StatisticsService>().GetQuestionDifficulty(all);
        HardList.ItemsSource = hard.Select(h => $"{(h.IsHard ? "[СКЛАДНЕ] " : "")}{h.TestTitle}: {h.QuestionText} — {h.WrongRatePercent}% ({h.Attempts})").ToList();
    }

    private void OnFilter(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => Refresh();

    private void Save(Action<string> export, string filter)
    {
        var dlg = new SaveFileDialog { Filter = filter };
        if (dlg.ShowDialog() != true) return;
        export(dlg.FileName);
        MessageBox.Show("Збережено.");
    }

    private void OnCsv(object sender, RoutedEventArgs e) =>
        Save(p => _sp.GetRequiredService<IReportExportService>().ExportResultsCsv(CurrentRows(), p), "CSV|*.csv");

    private void OnExcel(object sender, RoutedEventArgs e) =>
        Save(p => _sp.GetRequiredService<IReportExportService>().ExportResultsExcel(CurrentRows(), p), "Excel|*.xlsx");

    private void OnPdf(object sender, RoutedEventArgs e) =>
        Save(p => _sp.GetRequiredService<IReportExportService>().ExportReportPdf(
            new ReportData("Звіт", "Журнал (тиждень 6)", CurrentRows(),
                _sp.GetRequiredService<StatisticsService>().GetQuestionDifficulty(_sp.GetRequiredService<IResultRepository>().GetAll())), p), "PDF|*.pdf");
}
