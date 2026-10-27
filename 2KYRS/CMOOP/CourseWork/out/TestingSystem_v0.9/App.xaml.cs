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

/// <summary>Название текущего провайдера хранилища (показывается в индикаторе подключения).</summary>
public sealed record StorageInfo(string Name);

/// <summary>Набор сервисов приложения: окна получают его целиком и работают только с интерфейсами/сервисами.</summary>
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

    /// <summary>
    /// ===================== ТОЧКА ВХОДА / COMPOSITION ROOT / DI =====================
    /// Единственное место, где выбираются конкретные реализации.
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var sc = new ServiceCollection();

        // ---------- ХРАНИЛИЩЕ: сейчас — оперативная память ----------
        sc.AddSingleton<IUserRepository, InMemoryUserRepository>();
        sc.AddSingleton<IGroupRepository, InMemoryGroupRepository>();
        sc.AddSingleton<ICategoryRepository, InMemoryCategoryRepository>();
        sc.AddSingleton<ITestRepository, InMemoryTestRepository>();
        sc.AddSingleton<ILectureRepository, InMemoryLectureRepository>();
        sc.AddSingleton<IResultRepository, InMemoryResultRepository>();
        sc.AddSingleton(new StorageInfo("In-Memory"));

        // ---------- ПЕРЕХОД НА SQLite / PostgreSQL / EF Core ----------
        // Меняются ТОЛЬКО эти регистрации (классы пишутся на тех же интерфейсах), например:
        //   sc.AddDbContext<AppDbContext>(o => o.UseSqlite("Data Source=testing.db"));  // или UseNpgsql(...)
        //   sc.AddScoped<IUserRepository,   EfUserRepository>();
        //   sc.AddScoped<ITestRepository,   EfTestRepository>();
        //   sc.AddScoped<IResultRepository, EfResultRepository>();  ... и т.д.
        //   sc.AddSingleton(new StorageInfo("SQLite"));
        // GradingEngine, TestSessionService, StatisticsService, AuthService и все окна остаются без изменений.

        // ---------- сервисы и бизнес-логика (не зависят от хранилища) ----------
        sc.AddSingleton<GradingEngine>();
        sc.AddSingleton<IAuthService, AuthService>();
        sc.AddSingleton<ILocalizationService, LocalizationService>();
        sc.AddSingleton<IThemeService, ThemeService>();
        sc.AddSingleton<TestSessionService>();
        sc.AddSingleton<StatisticsService>();
        sc.AddSingleton<IReportExportService, ReportExportService>();

        // ---------- лекції ----------
        sc.AddSingleton<ILectureService, LectureService>();

        // ---------- AI: провайдери — взаємозамінні стратегії; обирається в налаштуваннях, без змін коду ----------
        sc.AddSingleton(new HttpClient { Timeout = Timeout.InfiniteTimeSpan });   // таймаут задає AiClientRouter
        sc.AddSingleton<IAiSettingsStore, JsonAiSettingsStore>();
        sc.AddSingleton<IAiProvider, MockAiProvider>();              // офлайн-демо без ключа
        sc.AddSingleton<IAiProvider, OpenAiCompatibleProvider>();    // OpenAI, OpenRouter, Ollama, LM Studio...
        sc.AddSingleton<IAiProvider, AnthropicProvider>();           // Anthropic Messages API
        sc.AddSingleton<IAiProvider, GeminiProvider>();              // Google Gemini API
        sc.AddSingleton<IAiClient, AiClientRouter>();
        sc.AddSingleton<IAiAssistant, AiAssistant>();

        // ---------- MVVM-інфраструктура: діалоги, фабрика ViewModel, міст до редактора тестів ----------
        sc.AddSingleton<IDialogService>(_ => new WpfDialogService()
            .Register<LectureEditorViewModel, LectureEditorWindow>()
            .Register<AiSettingsViewModel, AiSettingsWindow>());
        sc.AddSingleton<IViewModelFactory, ViewModelFactory>();
        sc.AddSingleton<ITestEditorLauncher, WpfTestEditorLauncher>();

        // ---------- блокировка сеанса после бездействия (каждая зависимость — отдельная абстракция) ----------
        sc.AddSingleton(new InactivitySettings(Timeout: TimeSpan.FromMinutes(30), MaxUnlockAttempts: 5));
        //   Для проверки поставьте, например, TimeSpan.FromSeconds(20) — экран блокировки появится через 20 секунд.
        sc.AddSingleton<IClock, SystemClock>();
        sc.AddSingleton<IActivitySource, WpfInputActivitySource>();
        sc.AddTransient<ITicker>(_ => new DispatcherTicker(TimeSpan.FromSeconds(5)));
        sc.AddSingleton<IInactivityMonitor, InactivityMonitor>();
        sc.AddSingleton<ILockScreenPresenter, WpfLockScreenPresenter>();
        sc.AddSingleton<ISessionNavigator>(_ => this);          // навигацию выполняет App
        sc.AddSingleton<ISessionLockService, SessionLockService>();

        sc.AddSingleton<AppServices>();

        _provider = sc.BuildServiceProvider();

        // Демо-данные (для реальной БД — убрать или вызывать только для пустой базы)
        DemoDataSeeder.Seed(
            _provider.GetRequiredService<IUserRepository>(),
            _provider.GetRequiredService<IGroupRepository>(),
            _provider.GetRequiredService<ICategoryRepository>(),
            _provider.GetRequiredService<ITestRepository>(),
            _provider.GetRequiredService<IResultRepository>(),
            _provider.GetRequiredService<ILectureRepository>(),
            _provider.GetRequiredService<GradingEngine>());

        _services = _provider.GetRequiredService<AppServices>();

        TExtension.Service = _services.Localization;   // нужно только XAML-расширению {loc:T ...}
        _services.Theme.SetTheme(AppTheme.Light);

        ShowLogin();
    }

    public void ShowLogin() => new LoginWindow(_services, ShowWorkspace).Show();

    /// <summary>
    /// ISessionNavigator: принудительный выход (например, с экрана блокировки). Завершает сеанс, показывает вход
    /// и закрывает все рабочие окна и диалоги — сначала дочерние, затем остальные. Экран блокировки закрывает сам себя.
    /// </summary>
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

    /// <summary>Маршрутизация по роли: ученик — кабинет ученика, учитель и админ — рабочее место преподавателя.</summary>
    private void ShowWorkspace(User user)
    {
        Window w = user.Role == Role.Student
            ? new StudentWindow(_services, ShowLogin)
            : new TeacherWindow(_services, ShowLogin);
        w.Show();
    }
}
