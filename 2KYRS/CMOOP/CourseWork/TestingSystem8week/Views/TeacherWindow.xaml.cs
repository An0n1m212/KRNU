using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Views;

/// <summary>Тиждень 8: + AI-помічник і налаштування (ТЗ п.3.7).</summary>
public partial class TeacherWindow : Window
{
    private readonly App _app;
    private readonly ServiceProvider _sp;

    public TeacherWindow(App app, ServiceProvider sp)
    {
        InitializeComponent();
        _app = app;
        _sp = sp;
        LangBox.ItemsSource = Enum.GetValues<AppLanguage>();
        LangBox.SelectedItem = _sp.GetRequiredService<ILocalizationService>().CurrentLanguage;
        ThemeBox.ItemsSource = Enum.GetValues<AppTheme>();
        ThemeBox.SelectedItem = _sp.GetRequiredService<IThemeService>().CurrentTheme;
        Refresh();
    }

    private IAuthService Auth => _sp.GetRequiredService<IAuthService>();
    private ITestRepository Tests => _sp.GetRequiredService<ITestRepository>();
    private IGroupRepository Groups => _sp.GetRequiredService<IGroupRepository>();

    private void Refresh() => TestsList.ItemsSource = Tests.GetAll();

    private void OnNew(object sender, RoutedEventArgs e)
    {
        var me = Auth.GetCurrentUser();
        var editor = new TestEditorWindow(Tests, Groups, new Test { AuthorTeacherId = me?.Id ?? 0 });
        editor.Closed += (_, _) => Refresh();
        editor.Show();
    }

    private void OnEdit(object sender, RoutedEventArgs e)
    {
        if (TestsList.SelectedItem is not Test t) return;
        var editor = new TestEditorWindow(Tests, Groups, t.DeepClone());
        editor.Closed += (_, _) => Refresh();
        editor.Show();
    }

    private void OnDelete(object sender, RoutedEventArgs e)
    {
        if (TestsList.SelectedItem is not Test t) return;
        Tests.Delete(t.Id);
        Refresh();
    }

    private void OnReview(object sender, RoutedEventArgs e) => new ReviewQueueWindow(_sp).Show();
    private void OnJournal(object sender, RoutedEventArgs e) => new JournalWindow(_sp).Show();
    private void OnAnalytics(object sender, RoutedEventArgs e) => new AnalyticsWindow(_sp).Show();
    private void OnLectures(object sender, RoutedEventArgs e) => new LecturesView(_sp).Show();
    private void OnAi(object sender, RoutedEventArgs e) => new AiAssistantWindow(_sp).Show();
    private void OnAiSettings(object sender, RoutedEventArgs e) => new AiSettingsWindow(_sp).Show();

    private void OnGroups(object sender, RoutedEventArgs e)
    {
        if (Auth.GetCurrentUser()?.Role != Role.Admin)
        {
            MessageBox.Show("Керування групами — лише адміністратор (ТЗ п.3.9).");
            return;
        }
        new GroupsView(_sp).Show();
    }

    private void OnLang(object sender, SelectionChangedEventArgs e)
    {
        if (LangBox.SelectedItem is AppLanguage lang)
            _sp.GetRequiredService<ILocalizationService>().SetLanguage(lang);
    }

    private void OnTheme(object sender, SelectionChangedEventArgs e)
    {
        if (ThemeBox.SelectedItem is AppTheme theme)
            _sp.GetRequiredService<IThemeService>().SetTheme(theme);
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
