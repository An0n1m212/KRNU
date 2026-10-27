using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;
using TestingSystem.Infrastructure;
using TestingSystem.Models;
using TestingSystem.Persistence;
using TestingSystem.Services;
using TestingSystem.Views;

namespace TestingSystem;

/// <summary>
/// Тиждень 7 (ТЗ п.3.6, п.3.9, п.3.2.3, п.3.1.4): лекції, групи, локалізація/теми, блокування.
/// Фінал додасть лише AI-шар і MVVM-інфраструктуру (див. фінальний `App.xaml.cs`).
/// </summary>
public partial class App : Application, ISessionNavigator
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
        sc.AddSingleton<ILectureRepository, InMemoryLectureRepository>();
        sc.AddSingleton<IAuthService, AuthService>();
        sc.AddSingleton<ILocalizationService, LocalizationService>();
        sc.AddSingleton<IThemeService, ThemeService>();
        sc.AddSingleton<GradingEngine>();
        sc.AddSingleton<TestSessionService>();
        sc.AddSingleton<StatisticsService>();
        sc.AddSingleton<IReportExportService, ReportExportService>();
        sc.AddSingleton<ILectureService, LectureService>();

        // Блокування після 30 хв бездіяльності (ТЗ п.3.2.3).
        sc.AddSingleton(new InactivitySettings(Timeout: TimeSpan.FromMinutes(30), MaxUnlockAttempts: 5));
        sc.AddSingleton<IClock, SystemClock>();
        sc.AddSingleton<IActivitySource, WpfInputActivitySource>();
        sc.AddTransient<ITicker>(_ => new DispatcherTicker(TimeSpan.FromSeconds(5)));
        sc.AddSingleton<IInactivityMonitor, InactivityMonitor>();
        sc.AddSingleton<ILockScreenPresenter, SimpleLockPresenter>();
        sc.AddSingleton<ISessionNavigator>(_ => this);
        sc.AddSingleton<ISessionLockService, SessionLockService>();

        _provider = sc.BuildServiceProvider();

        DemoDataSeeder.Seed(
            _provider.GetRequiredService<IUserRepository>(),
            _provider.GetRequiredService<IGroupRepository>(),
            _provider.GetRequiredService<ICategoryRepository>(),
            _provider.GetRequiredService<ITestRepository>(),
            _provider.GetRequiredService<IResultRepository>(),
            _provider.GetRequiredService<ILectureRepository>(),
            _provider.GetRequiredService<GradingEngine>());

        // Активуємо підписки блокування + мову/тему.
        _ = _provider.GetRequiredService<ISessionLockService>();
        TExtension.Service = _provider.GetRequiredService<ILocalizationService>();
        _provider.GetRequiredService<IThemeService>().SetTheme(AppTheme.Light);

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
            new StudentWindow(this, _provider!).Show();
        else
            new TeacherWindow(this, _provider!).Show();
    }

    public void NavigateToLogin()
    {
        _provider!.GetRequiredService<IAuthService>().Logout();
        var toClose = Windows.OfType<Window>().Where(w => w is not SessionLockWindow && w is not LoginWindow).ToList();
        ShowLogin();
        foreach (var w in toClose) w.Close();
    }

    public ServiceProvider Provider => _provider!;
}
