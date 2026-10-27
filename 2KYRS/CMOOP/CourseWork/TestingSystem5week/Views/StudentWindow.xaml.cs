using System.Windows;
using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Views;

/// <summary>
/// Тиждень 5: картки доступних тестів (ТЗ п.3.4.1) + старт через пін-діалог + «Мої результати».
/// Кабінет викладача — без змін (конструктор тижня 4).
/// </summary>
public partial class StudentWindow : Window
{
    private readonly IAuthService _auth;
    private readonly TestSessionService _sessions;
    private readonly IResultRepository _results;
    private readonly ITestRepository _tests;
    private readonly Action _onLogout;

    public StudentWindow(IAuthService auth, TestSessionService sessions, IResultRepository results, ITestRepository tests, Action onLogout)
    {
        InitializeComponent();
        _auth = auth;
        _sessions = sessions;
        _results = results;
        _tests = tests;
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
        ResultsList.ItemsSource = _results.GetByStudent(me.Id)
            .OrderByDescending(r => r.Date)
            .Select(r => $"{r.Date:g} — {TestTitle(r.TestId)}: {r.TotalScore}/{r.MaxScore} ({r.Percentage}%) — {r.Status}").ToList();
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
            var dlg = new AccessCodeDialog();
            dlg.Owner = this;
            if (dlg.ShowDialog() != true) return;
            pin = dlg.AccessCode;
        }

        var (error, controller) = _sessions.TryStart(me, test.Id, pin);
        if (error != StartError.None || controller is null)
        {
            MessageBox.Show(error switch
            {
                StartError.NoAttemptsLeft => "Спроби вичерпано.",
                StartError.WrongAccessCode => "Невірний пін-код.",
                StartError.NoQuestions => "Порожній тест.",
                StartError.NotAssigned => "Тест не призначено вашій групі.",
                _ => "Не вдалося почати."
            });
            return;
        }

        var run = new TestRunWindow(controller);
        run.Closed += (_, _) => Refresh();
        run.Show();
    }

    private void OnRefresh(object sender, RoutedEventArgs e) => Refresh();

    private void OnLogout(object sender, RoutedEventArgs e)
    {
        _auth.Logout();
        _onLogout();
        Close();
    }
}
