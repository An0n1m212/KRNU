using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Persistence;
using TestingSystem.Services;
using TestingSystem.Views;

namespace TestingSystem;

/// <summary>
/// Тиждень 6 (ТЗ п.3.5, п.3.8, розд.6 етап 5): перевірка + журнал/аналітика/експорт.
/// Локалізація тут — лише як залежність експорту (повноцінно UA/RU/EN + теми — тиждень 7).
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
        sc.AddSingleton<ILocalizationService, LocalizationService>();
        sc.AddSingleton<GradingEngine>();
        sc.AddSingleton<TestSessionService>();
        sc.AddSingleton<StatisticsService>();
        sc.AddSingleton<IReportExportService, ReportExportService>();
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
                _provider.GetRequiredService<IReportExportService>(),
                ShowLogin).Show();
        else
            new TeacherWindow(this, _provider!).Show();
    }

    public ServiceProvider Provider => _provider!;
}
