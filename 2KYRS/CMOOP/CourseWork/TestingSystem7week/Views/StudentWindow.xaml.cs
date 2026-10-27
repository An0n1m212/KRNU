using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Views;

/// <summary>Тиждень 7: + лекції, мова/тема, блокування.</summary>
public partial class StudentWindow : Window
{
    private readonly App _app;
    private readonly ServiceProvider _sp;

    public StudentWindow(App app, ServiceProvider sp)
    {
        InitializeComponent();
        _app = app;
        _sp = sp;
        LangBox.ItemsSource = Enum.GetValues<AppLanguage>();
        LangBox.SelectedItem = Loc.CurrentLanguage;
        ThemeBox.ItemsSource = Enum.GetValues<AppTheme>();
        ThemeBox.SelectedItem = _sp.GetRequiredService<IThemeService>().CurrentTheme;
        var u = Auth.GetCurrentUser();
        UserText.Text = u is null ? "—" : $"{u.FullName} ({u.Username})";
        Refresh();
    }

    private IAuthService Auth => _sp.GetRequiredService<IAuthService>();
    private TestSessionService Sessions => _sp.GetRequiredService<TestSessionService>();
    private IResultRepository Results => _sp.GetRequiredService<IResultRepository>();
    private ITestRepository Tests => _sp.GetRequiredService<ITestRepository>();
    private IReportExportService Export => _sp.GetRequiredService<IReportExportService>();
    private ILocalizationService Loc => _sp.GetRequiredService<ILocalizationService>();

    private void Refresh()
    {
        var me = Auth.GetCurrentUser();
        if (me is null) return;
        var available = Sessions.GetAvailableTests(me);
        TestsList.ItemsSource = available.Select(t =>
            $"{t.Title} — {t.TimeLimitMinutes} хв • {t.Questions.Count} пит. • прохідний {t.PassScorePercent}% • спроби {Sessions.GetAttemptsUsed(me.Id, t.Id)}/{(t.MaxAttempts == 0 ? "∞" : t.MaxAttempts)}" +
            (string.IsNullOrWhiteSpace(t.AccessCode) ? "" : " • PIN")).ToList();
        TestsList.Tag = available.ToList();
        var mine = Results.GetByStudent(me.Id).OrderByDescending(r => r.Date).ToList();
        ResultsList.Tag = mine;
        ResultsList.ItemsSource = mine.Select(r => $"{r.Date:g} — {Tests.GetById(r.TestId)?.Title}: {r.TotalScore}/{r.MaxScore} ({r.Percentage}%) — {r.Status}").ToList();
    }

    private void OnStart(object sender, RoutedEventArgs e)
    {
        var me = Auth.GetCurrentUser();
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
        var (error, controller) = Sessions.TryStart(me, test.Id, pin);
        if (error != StartError.None || controller is null) { MessageBox.Show("Не вдалося почати: " + error); return; }
        var run = new TestRunWindow(controller);
        run.Closed += (_, _) => Refresh();
        run.Show();
    }

    private void OnDetails(object sender, RoutedEventArgs e)
    {
        if (ResultsList.SelectedIndex < 0) return;
        new ResultDetailsWindow(Tests, ((List<Result>)ResultsList.Tag)[ResultsList.SelectedIndex]) { Owner = this }.ShowDialog();
    }

    private void OnCertificate(object sender, RoutedEventArgs e)
    {
        if (ResultsList.SelectedIndex < 0) return;
        var r = ((List<Result>)ResultsList.Tag)[ResultsList.SelectedIndex];
        if (r.Status == ResultStatus.PendingTeacherReview || !r.IsPassed) { MessageBox.Show("Сертифікат — лише за успішно складений і перевірений тест."); return; }
        var me = Auth.GetCurrentUser();
        var dlg = new SaveFileDialog { Filter = "PDF|*.pdf", FileName = $"certificate_{r.Id}.pdf" };
        if (dlg.ShowDialog() != true) return;
        Export.ExportCertificatePdf(new CertificateData(me?.FullName ?? "", Tests.GetById(r.TestId)?.Title ?? "", r.Percentage, r.Date, $"TS-{r.Id:D6}"), dlg.FileName);
        MessageBox.Show("Збережено.");
    }

    private void OnLectures(object sender, RoutedEventArgs e) => new StudentLecturesView(_sp, Sessions, Tests) { Owner = this }.Show();

    private void OnLang(object sender, SelectionChangedEventArgs e)
    {
        if (LangBox.SelectedItem is AppLanguage lang) Loc.SetLanguage(lang);
    }

    private void OnTheme(object sender, SelectionChangedEventArgs e)
    {
        if (ThemeBox.SelectedItem is AppTheme theme) _sp.GetRequiredService<IThemeService>().SetTheme(theme);
    }

    private void OnLock(object sender, RoutedEventArgs e) =>
        _sp.GetRequiredService<ISessionLockService>().Lock();

    private void OnLogout(object sender, RoutedEventArgs e)
    {
        Auth.Logout();
        _app.ShowLogin();
        Close();
    }
}
