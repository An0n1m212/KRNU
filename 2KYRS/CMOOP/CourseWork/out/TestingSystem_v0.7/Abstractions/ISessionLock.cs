using TestingSystem.Models;

namespace TestingSystem.Abstractions;

/// <summary>Настройки блокировки по бездействию (меняются в одном месте — Composition Root).</summary>
public sealed record InactivitySettings(TimeSpan Timeout, int MaxUnlockAttempts = 5);

/// <summary>Источник текущего времени (подменяется в тестах — Dependency Inversion).</summary>
public interface IClock
{
    DateTime Now { get; }
}

/// <summary>Периодический «тик» (в WPF — DispatcherTimer, в тестах — ручной).</summary>
public interface ITicker
{
    event EventHandler? Tick;
    void Start();
    void Stop();
}

/// <summary>Источник событий пользовательской активности (мышь, клавиатура, касания).</summary>
public interface IActivitySource
{
    event EventHandler? ActivityDetected;
}

/// <summary>Отслеживает бездействие и сообщает, что лимит времени исчерпан.</summary>
public interface IInactivityMonitor
{
    TimeSpan Timeout { get; }
    bool IsRunning { get; }

    /// <summary>Запускает (или перезапускает) отсчёт бездействия.</summary>
    void Start();
    void Stop();

    /// <summary>Приостанавливает блокировку (например, на время прохождения теста): using (monitor.Suspend()) { ... }</summary>
    IDisposable Suspend();

    /// <summary>Сработает один раз; после него монитор остановлен до следующего Start().</summary>
    event EventHandler? TimedOut;
}

public enum UnlockResult { Success, WrongPassword, TooManyAttempts }

/// <summary>Бизнес-логика блокировки сеанса: когда блокировать, как снимать, когда принудительно выходить.</summary>
public interface ISessionLockService
{
    bool IsLocked { get; }
    int AttemptsLeft { get; }

    /// <summary>Блокирует сеанс немедленно (вызывается и по таймауту бездействия).</summary>
    void Lock();

    /// <summary>Повторная аутентификация текущего пользователя.</summary>
    UnlockResult TryUnlock(string password);

    /// <summary>Выйти из аккаунта прямо с экрана блокировки.</summary>
    void ForceLogout();
}

/// <summary>Показывает экран блокировки. Вызов блокирующий: возвращается, когда сеанс разблокирован или завершён.</summary>
public interface ILockScreenPresenter
{
    void ShowLockScreen(ISessionLockService lockService);
}

/// <summary>Навигация между состояниями приложения (убирает зависимость сервисов от конкретных окон).</summary>
public interface ISessionNavigator
{
    /// <summary>Завершает сеанс, закрывает рабочие окна и показывает экран входа.</summary>
    void NavigateToLogin();
}
