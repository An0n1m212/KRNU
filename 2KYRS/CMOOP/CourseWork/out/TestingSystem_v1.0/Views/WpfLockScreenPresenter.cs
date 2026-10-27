using TestingSystem.Abstractions;
using TestingSystem.ViewModels;

namespace TestingSystem.Views;

/// <summary>
/// WPF-реализация ILockScreenPresenter: собирает ViewModel и показывает окно модально.
/// Единственное место, знающее, что экран блокировки — это SessionLockWindow (замена = новая реализация интерфейса).
/// </summary>
public sealed class WpfLockScreenPresenter : ILockScreenPresenter
{
    private readonly IAuthService _auth;
    private readonly ILocalizationService _loc;
    private readonly InactivitySettings _settings;

    public WpfLockScreenPresenter(IAuthService auth, ILocalizationService loc, InactivitySettings settings)
    {
        _auth = auth;
        _loc = loc;
        _settings = settings;
    }

    public void ShowLockScreen(ISessionLockService lockService)
    {
        var user = _auth.GetCurrentUser();
        if (user is null) return;

        var viewModel = new SessionLockViewModel(lockService, user, _settings.Timeout, _loc);
        new SessionLockWindow(viewModel).ShowDialog();
    }
}
