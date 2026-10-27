using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Views;

/// <summary>Тиждень 6: до конструктора додано перевірку, журнал, аналітику (ТЗ п.3.5, п.3.8).</summary>
public partial class TeacherWindow : Window
{
    private readonly App _app;
    private readonly ServiceProvider _sp;

    public TeacherWindow(App app, ServiceProvider sp)
    {
        InitializeComponent();
        _app = app;
        _sp = sp;
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

    private void OnLogout(object sender, RoutedEventArgs e)
    {
        Auth.Logout();
        _app.ShowLogin();
        Close();
    }
}
