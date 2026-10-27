using System.Windows;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;
using TestingSystem.Infrastructure;
using TestingSystem.Models;
using TestingSystem.Persistence;
using TestingSystem.Services;
using TestingSystem.Services.Ai;
using TestingSystem.ViewModels;
using TestingSystem.Views;

namespace TestingSystem;

/// <summary>Назва провайдера сховища (індикатор підключення в шапці).</summary>
public sealed record StorageInfo(string Name);

/// <summary>Набір сервісів, доступних вікнам цієї версії.</summary>
public sealed record AppServices(
    IUserRepository Users,
    IGroupRepository Groups,
    ICategoryRepository Categories,
    ITestRepository Tests,
    IResultRepository Results,
    IAuthService Auth,
    ILocalizationService Localization,
    IThemeService Theme,
    GradingEngine Grading,
    TestSessionService Sessions,
    StatisticsService Stats,
    IReportExportService Export,
    IInactivityMonitor Inactivity,
    ISessionLockService SessionLock,
    IAiAssistant AiAssistant,
    IViewModelFactory ViewModels,
    StorageInfo Storage);

public partial class App : Application, ISessionNavigator
{
    private ServiceProvider _provider = null!;
    private AppServices _services = null!;

    /// <summary>Composition Root: єдине місце, де вибираються реалізації.</summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var sc = new ServiceCollection();

        // ---------- сховище (in-memory) ----------
        sc.AddSingleton<IUserRepository, InMemoryUserRepository>();
        sc.AddSingleton<IGroupRepository, InMemoryGroupRepository>();
        sc.AddSingleton<ICategoryRepository, InMemoryCategoryRepository>();
        sc.AddSingleton<ITestRepository, InMemoryTestRepository>();
        sc.AddSingleton<ILectureRepository, InMemoryLectureRepository>();
        sc.AddSingleton<IResultRepository, InMemoryResultRepository>();
        sc.AddSingleton(new StorageInfo("In-Memory"));

        // ---------- сервіси та бізнес-логіка ----------
        sc.AddSingleton<GradingEngine>();
        sc.AddSingleton<IAuthService, AuthService>();
        sc.AddSingleton<ILocalizationService, LocalizationService>();
        sc.AddSingleton<IThemeService, ThemeService>();
        sc.AddSingleton<TestSessionService>();
        sc.AddSingleton<StatisticsService>();
        sc.AddSingleton<IReportExportService, ReportExportService>();
        sc.AddSingleton<ILectureService, LectureService>();

        // ---------- AI: взаємозамінні провайдери-стратегії ----------
        sc.AddSingleton(new HttpClient { Timeout = Timeout.InfiniteTimeSpan });   // таймаут задає AiClientRouter
        sc.AddSingleton<IAiSettingsStore, JsonAiSettingsStore>();
        sc.AddSingleton<IAiProvider, MockAiProvider>();
        sc.AddSingleton<IAiProvider, OpenAiCompatibleProvider>();
        sc.AddSingleton<IAiProvider, AnthropicProvider>();
        sc.AddSingleton<IAiClient, AiClientRouter>();
        sc.AddSingleton<IAiAssistant, AiAssistant>();

        // ---------- MVVM-інфраструктура ----------
        sc.AddSingleton<IDialogService>(_ => new WpfDialogService()
            .Register<LectureEditorViewModel, LectureEditorWindow>()
            .Register<AiSettingsViewModel, AiSettingsWindow>());
        sc.AddSingleton<IViewModelFactory, ViewModelFactory>();
        sc.AddSingleton<ITestEditorLauncher, WpfTestEditorLauncher>();

        // ---------- блокування сеансу після бездіяльності ----------
        sc.AddSingleton(new InactivitySettings(Timeout: TimeSpan.FromMinutes(30), MaxUnlockAttempts: 5));
        sc.AddSingleton<IClock, SystemClock>();
        sc.AddSingleton<IActivitySource, WpfInputActivitySource>();
        sc.AddTransient<ITicker>(_ => new DispatcherTicker(TimeSpan.FromSeconds(5)));
        sc.AddSingleton<IInactivityMonitor, InactivityMonitor>();
        sc.AddSingleton<ILockScreenPresenter, WpfLockScreenPresenter>();
        sc.AddSingleton<ISessionNavigator>(_ => this);
        sc.AddSingleton<ISessionLockService, SessionLockService>();

        sc.AddSingleton<AppServices>();

        _provider = sc.BuildServiceProvider();

        DemoDataSeeder.Seed(
            _provider.GetRequiredService<IUserRepository>(),
            _provider.GetRequiredService<IGroupRepository>(),
            _provider.GetRequiredService<ICategoryRepository>(),
            _provider.GetRequiredService<ITestRepository>(),
            _provider.GetRequiredService<IResultRepository>(),
            _provider.GetRequiredService<ILectureRepository>(),
            _provider.GetRequiredService<GradingEngine>());

        _services = _provider.GetRequiredService<AppServices>();

        TExtension.Service = _services.Localization;   // потрібно лише XAML-розширенню {loc:T ...}
        _services.Theme.SetTheme(AppTheme.Light);

        ShowLogin();
    }

    public void ShowLogin() => new LoginWindow(_services, ShowWorkspace).Show();

    /// <summary>ISessionNavigator: примусовий вихід (наприклад, з екрана блокування).</summary>
    public void NavigateToLogin()
    {
        _services.Auth.Logout();

        var toClose = Windows.OfType<Window>()
            .Where(w => w is not SessionLockWindow && w is not LoginWindow)
            .OrderByDescending(w => w.Owner != null)
            .ToList();

        ShowLogin();
        foreach (var window in toClose) window.Close();
    }

    /// <summary>Маршрутизація за роллю: студент - кабінет студента, викладач і адмін - місце викладача.</summary>
    private void ShowWorkspace(User user)
    {
        Window w = user.Role == Role.Student
            ? new StudentWindow(_services, ShowLogin)
            : new TeacherWindow(_services, ShowLogin);
        w.Show();
    }
}
