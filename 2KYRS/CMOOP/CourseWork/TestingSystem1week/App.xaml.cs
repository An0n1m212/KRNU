using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Persistence;
using TestingSystem.Views;

namespace TestingSystem;

/// <summary>
/// Тиждень 1 (ТЗ п.1, розд.6 етап 1): моделі даних + шари + DI.
/// Composition Root реєструє лише in-memory сховище (користувачі, групи).
/// Auth, тести, журнал, лекції, AI — у наступних тижнях.
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _provider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var sc = new ServiceCollection();
        // Шар Persistence за інтерфейсами з Abstractions (заміна на SQLite/EF Core — лише реєстрації тут).
        sc.AddSingleton<IUserRepository, InMemoryUserRepository>();
        sc.AddSingleton<IGroupRepository, InMemoryGroupRepository>();
        _provider = sc.BuildServiceProvider();

        // Мінімальні демо-дані тижня 1: лише групи (користувачі/тести — пізніше).
        var groups = _provider.GetRequiredService<IGroupRepository>();
        if (groups.GetAll().Count == 0)
        {
            groups.Save(new Group { Name = "КН-25-1" });
            groups.Save(new Group { Name = "ІБ-24-2" });
        }

        var users = _provider.GetRequiredService<IUserRepository>();
        new MainWindow(users, groups).Show();
    }
}
