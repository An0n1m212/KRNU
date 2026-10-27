using System.Windows;
using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Views;

/// <summary>Тиждень 2: спільне вікно з міткою ролі (розділення на кабінети — тиждень 5).</summary>
public partial class MainWindow : Window
{
    private readonly IAuthService _auth;
    private readonly IUserRepository _users;
    private readonly IGroupRepository _groups;
    private readonly Action _onLogout;

    public MainWindow(IAuthService auth, IUserRepository users, IGroupRepository groups, Action onLogout)
    {
        InitializeComponent();
        _auth = auth;
        _users = users;
        _groups = groups;
        _onLogout = onLogout;
        var u = _auth.GetCurrentUser();
        UserText.Text = u is null ? "—" : $"{u.FullName} ({u.Username}) — роль: {u.Role}";
        GroupsList.ItemsSource = _groups.GetAll();
        UsersList.ItemsSource = _users.GetAll();
    }

    private void OnLogout(object sender, RoutedEventArgs e)
    {
        _auth.Logout();
        _onLogout();
        Close();
    }
}
