using System.Windows;
using Microsoft.Win32;
using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Views;

/// <summary>Тиждень 6: розбір помилок + PDF-сертифікат (ТЗ п.3.8.3).</summary>
public partial class StudentWindow : Window
{
    private readonly IAuthService _auth;
    private readonly TestSessionService _sessions;
    private readonly IResultRepository _results;
    private readonly ITestRepository _tests;
    private readonly IReportExportService _export;
    private readonly Action _onLogout;

    public StudentWindow(IAuthService auth, TestSessionService sessions, IResultRepository results,
        ITestRepository tests, IReportExportService export, Action onLogout)
    {
        InitializeComponent();
        _auth = auth;
        _sessions = sessions;
        _results = results;
        _tests = tests;
        _export = export;
        _onLogout = onLogout;
        var u = _auth.GetCurrentUser();
        UserText.Text = u is null ? "—" : $"{u.FullName} ({u.Username})";
        Refresh();
    }

    private void Refresh()
    {
        var me = _auth.GetCurrentUser();
        if (me is null) return;
        var available = _sessions.GetAvailableTests(me);
        TestsList.ItemsSource = available.Select(t =>
            $"{t.Title} — {t.TimeLimitMinutes} хв • {t.Questions.Count} пит. • прохідний {t.PassScorePercent}% • спроби {_sessions.GetAttemptsUsed(me.Id, t.Id)}/{(t.MaxAttempts == 0 ? "∞" : t.MaxAttempts)}" +
            (string.IsNullOrWhiteSpace(t.AccessCode) ? "" : " • PIN")).ToList();
        TestsList.Tag = available.ToList();
        var mine = _results.GetByStudent(me.Id).OrderByDescending(r => r.Date).ToList();
        ResultsList.Tag = mine;
        ResultsList.ItemsSource = mine.Select(r => $"{r.Date:g} — {TestTitle(r.TestId)}: {r.TotalScore}/{r.MaxScore} ({r.Percentage}%) — {r.Status}").ToList();
    }

    private string TestTitle(int id) => _tests.GetById(id)?.Title ?? $"тест #{id}";

    private void OnStart(object sender, RoutedEventArgs e)
    {
        var me = _auth.GetCurrentUser();
        if (me is null || TestsList.SelectedIndex < 0) return;
        var available = (List<Test>)TestsList.Tag;
        var test = available[TestsList.SelectedIndex];

        string? pin = null;
        if (!string.IsNullOrWhiteSpace(test.AccessCode))
        {
            var dlg = new AccessCodeDialog { Owner = this };
            if (dlg.ShowDialog() != true) return;
            pin = dlg.AccessCode;
        }

        var (error, controller) = _sessions.TryStart(me, test.Id, pin);
        if (error != StartError.None || controller is null)
        {
            MessageBox.Show("Не вдалося почати: " + error);
            return;
        }
        var run = new TestRunWindow(controller);
        run.Closed += (_, _) => Refresh();
        run.Show();
    }

    private void OnDetails(object sender, RoutedEventArgs e)
    {
        if (ResultsList.SelectedIndex < 0) return;
        var mine = (List<Result>)ResultsList.Tag;
        new ResultDetailsWindow(_tests, mine[ResultsList.SelectedIndex]) { Owner = this }.ShowDialog();
    }

    private void OnCertificate(object sender, RoutedEventArgs e)
    {
        if (ResultsList.SelectedIndex < 0) return;
        var mine = (List<Result>)ResultsList.Tag;
        var r = mine[ResultsList.SelectedIndex];
        if (r.Status == ResultStatus.PendingTeacherReview || !r.IsPassed)
        {
            MessageBox.Show("Сертифікат — лише за успішно складений і перевірений тест.");
            return;
        }
        var me = _auth.GetCurrentUser();
        var dlg = new SaveFileDialog { Filter = "PDF|*.pdf", FileName = $"certificate_{r.Id}.pdf" };
        if (dlg.ShowDialog() != true) return;
        _export.ExportCertificatePdf(new CertificateData(
            me?.FullName ?? "", TestTitle(r.TestId), r.Percentage, r.Date, $"TS-{r.Id:D6}"), dlg.FileName);
        MessageBox.Show("Збережено.");
    }

    private void OnLogout(object sender, RoutedEventArgs e)
    {
        _auth.Logout();
        _onLogout();
        Close();
    }
}
