using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;
using TestingSystem.Infrastructure;
using TestingSystem.Models;
using TestingSystem.Persistence;
using TestingSystem.Services;
using TestingSystem.Views;

namespace TestingSystem;

/// <summary>Назва провайдера сховища (індикатор підключення в шапці).</summary>
public sealed record StorageInfo(string Name);

/// <summary>Набір сервісів, доступних вікнам цієї версії.</summary>
public sealed record AppServices(
    IUserRepository Users,
    IGroupRepository Groups,
    ICategoryRepository Categories,
    IAuthService Auth,
    ILocalizationService Localization,
    IThemeService Theme,
    StorageInfo Storage);

public partial class App : Application
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
        sc.AddSingleton(new StorageInfo("In-Memory"));

        // ---------- сервіси та бізнес-логіка ----------
        sc.AddSingleton<IAuthService, AuthService>();
        sc.AddSingleton<ILocalizationService, LocalizationService>();
        sc.AddSingleton<IThemeService, ThemeService>();

        sc.AddSingleton<AppServices>();

        _provider = sc.BuildServiceProvider();

        DemoDataSeeder.Seed(
            _provider.GetRequiredService<IUserRepository>(),
            _provider.GetRequiredService<IGroupRepository>(),
            _provider.GetRequiredService<ICategoryRepository>());

        _services = _provider.GetRequiredService<AppServices>();

        TExtension.Service = _services.Localization;   // потрібно лише XAML-розширенню {loc:T ...}
        _services.Theme.SetTheme(AppTheme.Light);

        ShowLogin();
    }

    public void ShowLogin() => new LoginWindow(_services, ShowWorkspace).Show();

    /// <summary>Маршрутизація за роллю: студент - кабінет студента, викладач і адмін - місце викладача.</summary>
    private void ShowWorkspace(User user)
    {
        Window w = user.Role == Role.Student
            ? new StudentWindow(_services, ShowLogin)
            : new TeacherWindow(_services, ShowLogin);
        w.Show();
    }
}
