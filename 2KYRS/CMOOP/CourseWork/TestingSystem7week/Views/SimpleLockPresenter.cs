using System.Windows;
using TestingSystem.Abstractions;

namespace TestingSystem.Views;

/// <summary>Тиждень 7: спрощений презентер екрана блокування (фінал — WpfLockScreenPresenter + ViewModel).</summary>
public sealed class SimpleLockPresenter : ILockScreenPresenter
{
    public void ShowLockScreen(ISessionLockService lockService)
    {
        var active = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
        new SessionLockWindow(lockService) { Owner = active }.ShowDialog();
    }
}
