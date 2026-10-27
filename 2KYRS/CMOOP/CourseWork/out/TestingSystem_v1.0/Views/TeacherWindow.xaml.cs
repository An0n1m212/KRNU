using System.Windows;
using TestingSystem.Models;
using TestingSystem.ViewModels;

namespace TestingSystem.Views;

/// <summary>Рабочее место преподавателя (роли Teacher и Admin).</summary>
public partial class TeacherWindow : Window
{
    private readonly AppServices _services;

    public TeacherWindow(AppServices services, Action onLogout)
    {
        InitializeComponent();
        _services = services;

        // Ограничение прав по роли: окно недоступно ученикам
        if (!services.Auth.IsInRole(Role.Teacher, Role.Admin))
            throw new InvalidOperationException("Access denied: Teacher or Admin role required.");

        // Каждый View обновляет данные при показе (событие Loaded), поэтому вкладки всегда актуальны
        TestsTab.Content = new TestsManagementView(services);
        // Нові вкладки — на MVVM: ViewModel створює фабрика з DI, View лише відображає
        LecturesTab.Content = new LecturesView(services.ViewModels.Create<LecturesViewModel>());
        AiTab.Content = new AiAssistantView(services.ViewModels.Create<AiAssistantViewModel>());
        ReviewTab.Content = new ReviewView(services);
        AnalyticsTab.Content = new AnalyticsView(services);
        // Керування групами — лише для адміна (викладачу вкладка прихована, View не створюється)
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
