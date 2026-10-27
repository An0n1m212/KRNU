using TestingSystem.Abstractions;
using TestingSystem.Infrastructure.Mvvm;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.ViewModels;

/// <summary>
/// ViewModel экрана блокировки. Не знает ни об окнах, ни о PasswordBox: только состояние и команды.
/// Бизнес-правила (проверка пароля, число попыток) выполняет ISessionLockService.
/// </summary>
public sealed class SessionLockViewModel : ViewModelBase
{
    private readonly ISessionLockService _lock;
    private readonly ILocalizationService _loc;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;

    public string DisplayName { get; }
    public string Message { get; }

    public string Password
    {
        get => _password;
        set
        {
            if (SetProperty(ref _password, value ?? string.Empty))
            {
                if (_errorMessage.Length > 0) ErrorMessage = string.Empty;   // убираем старую ошибку при вводе
                UnlockCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public RelayCommand UnlockCommand { get; }
    public RelayCommand LogoutCommand { get; }

    /// <summary>View закрывает окно по этому событию (ViewModel не знает о Window).</summary>
    public event EventHandler? CloseRequested;

    public SessionLockViewModel(ISessionLockService lockService, User user, TimeSpan timeout, ILocalizationService loc)
    {
        _lock = lockService;
        _loc = loc;

        DisplayName = $"{user.FullName} ({user.Username})";
        Message = loc.Format("Lock_Message", Math.Max(1, (int)Math.Round(timeout.TotalMinutes)));

        UnlockCommand = new RelayCommand(Unlock, () => _password.Length > 0);
        LogoutCommand = new RelayCommand(Logout);
    }

    private void Unlock()
    {
        switch (_lock.TryUnlock(Password))
        {
            case UnlockResult.Success:
                CloseRequested?.Invoke(this, EventArgs.Empty);
                break;

            case UnlockResult.WrongPassword:
                Password = string.Empty;
                ErrorMessage = _loc.Format("Lock_WrongPassword", _lock.AttemptsLeft);
                break;

            case UnlockResult.TooManyAttempts:        // сервис уже завершил сеанс и показал экран входа
                CloseRequested?.Invoke(this, EventArgs.Empty);
                break;
        }
    }

    private void Logout()
    {
        _lock.ForceLogout();
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}
