using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Persistence;
using TestingSystem.Services;
using TestingSystem.Views;

namespace TestingSystem;

/// <summary>
/// Тиждень 5 (ТЗ п.3.4, розд.6 етап 4): проходження + <see cref="GradingEngine"/>.
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
        sc.AddSingleton<IResultRepository, InMemoryResultRepository>();
        sc.AddSingleton<IAuthService, AuthService>();
        sc.AddSingleton<GradingEngine>();
        sc.AddSingleton<TestSessionService>();
        _provider = sc.BuildServiceProvider();

        DemoDataSeeder.Seed(
            _provider.GetRequiredService<IUserRepository>(),
            _provider.GetRequiredService<IGroupRepository>(),
            _provider.GetRequiredService<ICategoryRepository>(),
            _provider.GetRequiredService<ITestRepository>(),
            _provider.GetRequiredService<IResultRepository>(),
            _provider.GetRequiredService<GradingEngine>());

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
            new StudentWindow(
                _provider!.GetRequiredService<IAuthService>(),
                _provider.GetRequiredService<TestSessionService>(),
                _provider.GetRequiredService<IResultRepository>(),
                _provider.GetRequiredService<ITestRepository>(),
                ShowLogin).Show();
        else
            new TeacherWindow(
                _provider!.GetRequiredService<IAuthService>(),
                _provider.GetRequiredService<ITestRepository>(),
                _provider.GetRequiredService<IGroupRepository>(),
                ShowLogin).Show();
    }
}
