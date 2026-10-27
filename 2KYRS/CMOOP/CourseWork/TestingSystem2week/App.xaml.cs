using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Persistence;
using TestingSystem.Services;
using TestingSystem.Views;

namespace TestingSystem;

/// <summary>
/// Тиждень 2 (ТЗ п.3.2, розд.6 етап 2): архітектура + автентифікація + ролі + демо-дані.
/// Фінальний `App.xaml.cs` виросте з цього: додадуться репозиторії тестів/результатів/лекцій,
/// GradingEngine, TestSessionService, статистика, експорт, лекції, теми, блокування, AI.
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _provider;
    private IAuthService? _auth;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var sc = new ServiceCollection();
        sc.AddSingleton<IUserRepository, InMemoryUserRepository>();
        sc.AddSingleton<IGroupRepository, InMemoryGroupRepository>();
        sc.AddSingleton<IAuthService, AuthService>();
        _provider = sc.BuildServiceProvider();

        DemoDataSeeder.Seed(
            _provider.GetRequiredService<IUserRepository>(),
            _provider.GetRequiredService<IGroupRepository>());

        _auth = _provider.GetRequiredService<IAuthService>();
        ShowLogin();
    }

    public void ShowLogin()
    {
        var login = new LoginWindow(
            _provider!.GetRequiredService<IAuthService>(),
            _provider.GetRequiredService<IGroupRepository>(),
            ShowWorkspace, ShowRegister);
        login.Show();
    }

    private void ShowRegister()
    {
        var reg = new RegisterWindow(
            _provider!.GetRequiredService<IAuthService>(),
            _provider.GetRequiredService<IGroupRepository>(),
            ShowLogin);
        reg.Show();
    }

    private void ShowWorkspace(User user)
    {
        var main = new MainWindow(
            _provider!.GetRequiredService<IAuthService>(),
            _provider.GetRequiredService<IUserRepository>(),
            _provider.GetRequiredService<IGroupRepository>(),
            ShowLogin);
        main.Show();
    }
}
