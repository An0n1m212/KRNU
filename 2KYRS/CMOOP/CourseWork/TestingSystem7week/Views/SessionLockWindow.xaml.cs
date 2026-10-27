using System.Windows;
using TestingSystem.Abstractions;

namespace TestingSystem.Views;

/// <summary>Тиждень 7 (ТЗ п.3.2.3): пароль для розблокування, ліміт спроб, примусовий вихід.</summary>
public partial class SessionLockWindow : Window
{
    private readonly ISessionLockService _lock;

    public SessionLockWindow(ISessionLockService lockService)
    {
        InitializeComponent();
        _lock = lockService;
    }

    private void OnUnlock(object sender, RoutedEventArgs e)
    {
        var res = _lock.TryUnlock(PasswordBox.Password);
        if (res == UnlockResult.Success)
        {
            DialogResult = true;
            Close();
        }
        else if (res == UnlockResult.TooManyAttempts)
        {
            MessageBox.Show("Перевищено число спроб. Вихід.");
            Close();
        }
        else
        {
            ErrorText.Text = $"Невірний пароль. Залишилось спроб: {_lock.AttemptsLeft}.";
        }
    }

    private void OnLogout(object sender, RoutedEventArgs e)
    {
        _lock.ForceLogout();
        Close();
    }
}
