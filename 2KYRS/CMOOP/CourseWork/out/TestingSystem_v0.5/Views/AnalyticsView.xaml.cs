using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using TestingSystem.Models;

namespace TestingSystem.Views;

/// <summary>Журнал и аналитика: результаты с фильтром по группе/тесту, сложные вопросы, экспорт и отчёт.</summary>
public partial class AnalyticsView : UserControl
{
    private readonly AppServices _s;
    private IReadOnlyList<ResultRow> _rows = Array.Empty<ResultRow>();
    private IReadOnlyList<QuestionDifficulty> _hard = Array.Empty<QuestionDifficulty>();

    public AnalyticsView(AppServices services)
    {
        InitializeComponent();
        _s = services;

        Loaded += (_, _) =>
        {
            _s.Localization.LanguageChanged -= OnLanguageChanged;
            _s.Localization.LanguageChanged += OnLanguageChanged;
            ReloadFilters();
        };
        Unloaded += (_, _) => _s.Localization.LanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(object? sender, EventArgs e) => ReloadFilters();

    /// <summary>Заполняет фильтры (Id = 0 — служебный пункт «Все») и запускает перерасчёт таблиц.</summary>
    private void ReloadFilters()
    {
        var loc = _s.Localization;
        int? groupId = (GroupCombo.SelectedItem as Group)?.Id;
        int? testId = (TestCombo.SelectedItem as Test)?.Id;

        var groups = new List<Group> { new() { Id = 0, Name = loc["Filter_AllGroups"] } };
        groups.AddRange(_s.Groups.GetAll());
        var tests = new List<Test> { new() { Id = 0, Title = loc["Filter_AllTests"] } };
        tests.AddRange(_s.Tests.GetAll().Where(_s.Auth.CanManage));

        GroupCombo.ItemsSource = groups;
        TestCombo.ItemsSource = tests;
        GroupCombo.SelectedItem = groups.FirstOrDefault(g => g.Id == groupId) ?? groups[0];
        TestCombo.SelectedItem = tests.FirstOrDefault(t => t.Id == testId) ?? tests[0];
    }

    private void Filter_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (GroupCombo.SelectedItem is null || TestCombo.SelectedItem is null) return;
        Refresh();
    }

    private void Refresh()
    {
        var group = (Group)GroupCombo.SelectedItem;
        var test = (Test)TestCombo.SelectedItem;

        var manageable = _s.Tests.GetAll().Where(_s.Auth.CanManage).Select(t => t.Id).ToHashSet();
        var students = _s.Users.GetAll().ToDictionary(u => u.Id);

        var results = _s.Results.GetAll().Where(r => manageable.Contains(r.TestId));
        if (test.Id != 0) results = results.Where(r => r.TestId == test.Id);
        if (group.Id != 0) results = results.Where(r => students.TryGetValue(r.StudentId, out var u) && u.GroupId == group.Id);
        var list = results.ToList();

        _rows = new ResultRowFactory(_s).Build(list);
        _hard = _s.Stats.GetQuestionDifficulty(list);

        ResultsGrid.ItemsSource = _rows;
        HardGrid.ItemsSource = _hard;
    }

    private void ResultsGrid_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (ResultsGrid.SelectedItem is not ResultRow row) return;
        new ResultDetailsWindow(_s, row.Source, row.Test, showCorrect: true) { Owner = Window.GetWindow(this) }.ShowDialog();
    }

    // ---------------------------------------------------------------- экспорт

    private IReadOnlyList<ResultExportRow> ExportRows() => _rows.Select(r => r.ToExport()).ToList();

    private void ExportExcel_Click(object sender, RoutedEventArgs e)
        => Export("Excel|*.xlsx", "results.xlsx", path => _s.Export.ExportResultsExcel(ExportRows(), path));

    private void ExportCsv_Click(object sender, RoutedEventArgs e)
        => Export("CSV|*.csv", "results.csv", path => _s.Export.ExportResultsCsv(ExportRows(), path));

    private void Report_Click(object sender, RoutedEventArgs e)
    {
        var loc = _s.Localization;
        var group = (GroupCombo.SelectedItem as Group)?.Name;
        var test = (TestCombo.SelectedItem as Test)?.Title;
        var report = new ReportData(
            loc["Report_Title"],
            $"{loc["Lbl_Group"]} {group}; {loc["Lbl_Test"]} {test}",
            ExportRows(), _hard);

        Export("PDF|*.pdf", "report.pdf", path => _s.Export.ExportReportPdf(report, path));
    }

    private void Export(string filter, string defaultName, Action<string> action)
    {
        var loc = _s.Localization;
        if (_rows.Count == 0) { MessageBox.Show(loc["Msg_NothingToExport"]); return; }

        var dialog = new SaveFileDialog { Filter = filter, FileName = defaultName };
        if (dialog.ShowDialog(Window.GetWindow(this)) != true) return;

        try
        {
            action(dialog.FileName);
            MessageBox.Show(loc["Msg_Saved"]);
        }
        catch (Exception ex)   // IO-ошибки, файл занят Excel'ом и т.п.
        {
            MessageBox.Show($"{loc["Err_ExportFailed"]}\n{ex.Message}");
        }
    }
}
