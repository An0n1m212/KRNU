using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;
using TestingSystem.Infrastructure;
using TestingSystem.Models;

namespace TestingSystem.Views;

/// <summary>
/// Міст MVVM -> редактор тестів на code-behind. AppServices запитується в момент виклику (а не в конструкторі),
/// щоб не створювати циклічну залежність у DI. Коли редактор тестів буде переведено на MVVM, цей клас замінить IDialogService.
/// </summary>
public sealed class WpfTestEditorLauncher : ITestEditorLauncher
{
    private readonly IServiceProvider _provider;

    public WpfTestEditorLauncher(IServiceProvider provider) => _provider = provider;

    public bool? ShowTestEditor(Test? test)
    {
        var services = _provider.GetRequiredService<AppServices>();
        var window = new TestEditorWindow(services, test) { Owner = WindowHelper.ActiveWindow() };
        return window.ShowDialog();
    }
}
