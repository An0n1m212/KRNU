using System.Windows;
using System.Windows.Controls;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Views;

/// <summary>Данные карточки теста (проекция Test для отображения).</summary>
public record TestCard(
    Test Source, string Title, string Category, string Description,
    string TimeLine, string RulesLine, string AttemptsLine, string LockLine, bool CanStart);

/// <summary>Каталог доступных тестов (карточки) и запуск прохождения.</summary>
public partial class AvailableTestsView : UserControl
{
    private readonly AppServices _s;

    public AvailableTestsView(AppServices services)
    {
        InitializeComponent();
        _s = services;

        Loaded += (_, _) =>
        {
            _s.Localization.LanguageChanged -= OnLanguageChanged;
            _s.Localization.LanguageChanged += OnLanguageChanged;
            Reload();
        };
        Unloaded += (_, _) => _s.Localization.LanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(object? sender, EventArgs e) => Reload();

    private void Reload()
    {
        var user = _s.Auth.GetCurrentUser();
        if (user is null) return;

        var loc = _s.Localization;
        var categories = _s.Categories.GetAll().ToDictionary(c => c.Id, c => c.Name);

        var cards = _s.Sessions.GetAvailableTests(user).Select(t =>
        {
            int used = _s.Sessions.GetAttemptsUsed(user.Id, t.Id);
            bool hasAttempts = t.MaxAttempts == 0 || used < t.MaxAttempts;

            return new TestCard(
                t, t.Title,
                t.CategoryId is int cid && categories.TryGetValue(cid, out var cn) ? cn : "",
                t.Description,
                "⏱ " + (t.TimeLimitMinutes > 0 ? loc.Format("Card_Time", t.TimeLimitMinutes) : loc["Card_NoLimit"]),
                "🎯 " + loc.Format("Card_Rules", t.PassScorePercent, t.Questions.Count),
                "🔁 " + (t.MaxAttempts > 0 ? loc.Format("Card_Attempts", used, t.MaxAttempts) : loc.Format("Card_AttemptsUnlimited", used)),
                string.IsNullOrWhiteSpace(t.AccessCode) ? "" : "🔒 " + loc["Card_Protected"],
                hasAttempts);
        }).ToList();

        Cards.ItemsSource = cards;
        EmptyText.Visibility = cards.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void Start_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is not TestCard card) return;
        var user = _s.Auth.GetCurrentUser()!;
        var loc = _s.Localization;

        // Тест защищён пин-кодом — спрашиваем его до создания сессии
        string? code = null;
        if (!string.IsNullOrWhiteSpace(card.Source.AccessCode))
        {
            var dialog = new AccessCodeDialog { Owner = Window.GetWindow(this) };
            if (dialog.ShowDialog() != true) return;
            code = dialog.Code;
        }

        var (error, controller) = _s.Sessions.TryStart(user, card.Source.Id, code);
        if (error != StartError.None)
        {
            MessageBox.Show(loc[$"StartErr_{error}"]);
            Reload();
            return;
        }

        new TestRunWindow(_s, controller!) { Owner = Window.GetWindow(this) }.ShowDialog();
        Reload();   // обновить счётчик попыток
    }
}
