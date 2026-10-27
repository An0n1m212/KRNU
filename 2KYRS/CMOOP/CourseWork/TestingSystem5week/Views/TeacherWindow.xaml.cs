using System.Windows;
using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Views;

/// <summary>Тиждень 4: список тестів; редактор отримує ще й групи для призначення (ТЗ п.3.3.1).</summary>
public partial class TeacherWindow : Window
{
    private readonly IAuthService _auth;
    private readonly ITestRepository _tests;
    private readonly IGroupRepository _groups;
    private readonly Action _onLogout;

    public TeacherWindow(IAuthService auth, ITestRepository tests, IGroupRepository groups, Action onLogout)
    {
        InitializeComponent();
        _auth = auth;
        _tests = tests;
        _groups = groups;
        _onLogout = onLogout;
        Refresh();
    }

    private void Refresh() => TestsList.ItemsSource = _tests.GetAll();

    private void OnNew(object sender, RoutedEventArgs e)
    {
        var me = _auth.GetCurrentUser();
        var editor = new TestEditorWindow(_tests, _groups, new Test { AuthorTeacherId = me?.Id ?? 0 });
        editor.Closed += (_, _) => Refresh();
        editor.Show();
    }

    private void OnEdit(object sender, RoutedEventArgs e)
    {
        if (TestsList.SelectedItem is not Test t) return;
        var editor = new TestEditorWindow(_tests, _groups, t.DeepClone());
        editor.Closed += (_, _) => Refresh();
        editor.Show();
    }

    private void OnDelete(object sender, RoutedEventArgs e)
    {
        if (TestsList.SelectedItem is not Test t) return;
        _tests.Delete(t.Id);
        Refresh();
    }

    private void OnLogout(object sender, RoutedEventArgs e)
    {
        _auth.Logout();
        _onLogout();
        Close();
    }
}
