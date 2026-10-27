using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Persistence;
using TestingSystem.Services;
using TestingSystem.Views;

namespace TestingSystem;

/// <summary>
/// Тиждень 3 (ТЗ п.3.3, розд.6 етап 3, частина 1): до DI додано тести і категорії.
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _provider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var sc = new ServiceCollection();
        sc.AddSingleton<IUserRepository, InMemoryUserRepository>();
        sc.AddSingleton<IGroupRepository, InMemoryGroupRepository>();
        sc.AddSingleton<ICategoryRepository, InMemoryCategoryRepository>();
        sc.AddSingleton<ITestRepository, InMemoryTestRepository>();
        sc.AddSingleton<IAuthService, AuthService>();
        _provider = sc.BuildServiceProvider();

        DemoDataSeeder.Seed(
            _provider.GetRequiredService<IUserRepository>(),
            _provider.GetRequiredService<IGroupRepository>(),
            _provider.GetRequiredService<ICategoryRepository>(),
            _provider.GetRequiredService<ITestRepository>());

        ShowLogin();
    }

    public void ShowLogin()
    {
        new LoginWindow(
            _provider!.GetRequiredService<IAuthService>(),
            _provider.GetRequiredService<IGroupRepository>(),
            ShowWorkspace, ShowRegister).Show();
    }

    private void ShowRegister()
    {
        new RegisterWindow(
            _provider!.GetRequiredService<IAuthService>(),
            _provider.GetRequiredService<IGroupRepository>(),
            ShowLogin).Show();
    }

    private void ShowWorkspace(User user)
    {
        if (user.Role == Role.Student)
            new StudentWindow(_provider!.GetRequiredService<ITestRepository>(), ShowLogin).Show();
        else
            new TeacherWindow(
                _provider!.GetRequiredService<IAuthService>(),
                _provider.GetRequiredService<ITestRepository>(),
                _provider.GetRequiredService<ICategoryRepository>(),
                ShowLogin).Show();
    }
}
