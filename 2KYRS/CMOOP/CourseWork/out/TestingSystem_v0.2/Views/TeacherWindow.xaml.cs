using System.Windows;
using TestingSystem.Models;

namespace TestingSystem.Views;

public partial class TeacherWindow : Window
{
    private readonly AppServices _services;

    public TeacherWindow(AppServices services, Action onLogout)
    {
        InitializeComponent();
        _services = services;

        if (!services.Auth.IsInRole(Role.Teacher, Role.Admin))
            throw new InvalidOperationException("Access denied: Teacher or Admin role required.");

        WelcomeTab.Content = new System.Windows.Controls.TextBlock
        {
            Text = "v0.2: вхід, ролі та шапка працюють. Робочі вкладки зʼявляться у наступних версіях.",
            Margin = new Thickness(24), FontSize = 16, TextWrapping = TextWrapping.Wrap
        };

        Header.Init(services);
        Header.LogoutRequested += (_, _) =>
        {
            _services.Auth.Logout();
            onLogout();
            Close();
        };
    }
}
