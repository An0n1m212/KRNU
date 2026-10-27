using System.Windows;
using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Views;

/// <summary>Тиждень 3: список тестів + перехід у спрощений конструктор (попередник фінального TestEditorWindow).</summary>
public partial class TeacherWindow : Window
{
    private readonly IAuthService _auth;
    private readonly ITestRepository _tests;
    private readonly ICategoryRepository _categories;
    private readonly Action _onLogout;

    public TeacherWindow(IAuthService auth, ITestRepository tests, ICategoryRepository categories, Action onLogout)
    {
        InitializeComponent();
        _auth = auth;
        _tests = tests;
        _categories = categories;
        _onLogout = onLogout;
        Refresh();
    }

    private void Refresh() => TestsList.ItemsSource = _tests.GetAll();

    private void OnNew(object sender, RoutedEventArgs e)
    {
        var me = _auth.GetCurrentUser();
        var editor = new TestEditorWindow(_tests, _categories, new Test { AuthorTeacherId = me?.Id ?? 0 });
        editor.Closed += (_, _) => Refresh();
        editor.Show();
    }

    private void OnEdit(object sender, RoutedEventArgs e)
    {
        if (TestsList.SelectedItem is not Test t) return;
        var editor = new TestEditorWindow(_tests, _categories, t.DeepClone());
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
