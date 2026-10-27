using System.Windows;
using TestingSystem.Models;
using TestingSystem.ViewModels;

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
        LecturesTab.Content = new StudentLecturesView(services.ViewModels.Create<StudentLecturesViewModel>());
        ResultsTab.Content = new MyResultsView(services);
        ProfileTab.Content = new ProfileView(services);

        Header.Init(services);
        Header.ProfileRequested += (_, _) => Tabs.SelectedItem = ProfileTab;
        Header.LogoutRequested += (_, _) =>
        {
            _services.Auth.Logout();
            onLogout();
            Close();
        };
    }
}
