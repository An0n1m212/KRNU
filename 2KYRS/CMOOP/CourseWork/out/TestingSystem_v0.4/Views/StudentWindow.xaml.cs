using System.Windows;
using TestingSystem.Models;

namespace TestingSystem.Views;

public partial class StudentWindow : Window
{
    private readonly AppServices _services;

    public StudentWindow(AppServices services, Action onLogout)
    {
        InitializeComponent();
        _services = services;

        if (!services.Auth.IsInRole(Role.Student))
            throw new InvalidOperationException("Access denied: Student role required.");

        TestsTab.Content = new AvailableTestsView(services);
        ResultsTab.Content = new MyResultsView(services);

        Header.Init(services);
        Header.LogoutRequested += (_, _) =>
        {
            _services.Auth.Logout();
            onLogout();
            Close();
        };
    }
}
