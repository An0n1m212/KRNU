using System.Windows;
using TestingSystem.Models;
using TestingSystem.ViewModels;

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
        LecturesTab.Content = new LecturesView(services.ViewModels.Create<LecturesViewModel>());
        ReviewTab.Content = new ReviewView(services);
        AnalyticsTab.Content = new AnalyticsView(services);
        if (services.Auth.IsInRole(Role.Admin))
            GroupsTab.Content = new GroupsView(services);
        else
            GroupsTab.Visibility = Visibility.Collapsed;
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
