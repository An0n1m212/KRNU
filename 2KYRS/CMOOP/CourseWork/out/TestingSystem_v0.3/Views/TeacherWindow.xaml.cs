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

        TestsTab.Content = new TestsManagementView(services);

        Header.Init(services);
        Header.LogoutRequested += (_, _) =>
        {
            _services.Auth.Logout();
            onLogout();
            Close();
        };
    }
}
