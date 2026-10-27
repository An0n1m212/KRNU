using TestingSystem.Abstractions;

namespace TestingSystem.Services;

/// <summary>
/// Оркестрирует блокировку сеанса. Зависит только от абстракций (DIP):
/// монитор бездействия, сервис аутентификации, показ экрана блокировки и навигация.
/// Жизненный цикл монитора привязан к состоянию входа: пользователь вошёл — отсчёт идёт, вышел — остановлен.
/// </summary>
public sealed class SessionLockService : ISessionLockService
{
    private readonly IInactivityMonitor _monitor;
    private readonly IAuthService _auth;
    private readonly ILockScreenPresenter _presenter;
    private readonly ISessionNavigator _navigator;
    private readonly InactivitySettings _settings;

    private int _failedAttempts;

    public bool IsLocked { get; private set; }
    public int AttemptsLeft => Math.Max(0, _settings.MaxUnlockAttempts - _failedAttempts);

    public SessionLockService(IInactivityMonitor monitor, IAuthService auth, ILockScreenPresenter presenter,
                              ISessionNavigator navigator, InactivitySettings settings)
    {
        _monitor = monitor;
        _auth = auth;
        _presenter = presenter;
        _navigator = navigator;
        _settings = settings;

        _auth.CurrentUserChanged += OnCurrentUserChanged;
        _monitor.TimedOut += (_, _) => Lock();
    }

    private void OnCurrentUserChanged(object? sender, EventArgs e)
    {
        if (_auth.GetCurrentUser() is null) _monitor.Stop();
        else if (!IsLocked) _monitor.Start();
    }

    public void Lock()
    {
        if (IsLocked || _auth.GetCurrentUser() is null) return;

        IsLocked = true;
        _failedAttempts = 0;
        _monitor.Stop();

        try
        {
            _presenter.ShowLockScreen(this);      // блокирующий вызов: вернётся после разблокировки или выхода
        }
        finally
        {
            IsLocked = false;
            if (_auth.GetCurrentUser() != null) _monitor.Start();   // разблокировали — снова следим за бездействием
        }
    }

    public UnlockResult TryUnlock(string password)
    {
        if (!IsLocked) return UnlockResult.Success;

        if (_auth.Reauthenticate(password))
        {
            _failedAttempts = 0;
            return UnlockResult.Success;
        }

        _failedAttempts++;
        if (_failedAttempts >= _settings.MaxUnlockAttempts)
        {
            ForceLogout();                        // защита от подбора пароля
            return UnlockResult.TooManyAttempts;
        }
        return UnlockResult.WrongPassword;
    }

    public void ForceLogout() => _navigator.NavigateToLogin();
}
