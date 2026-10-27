using System.ComponentModel;
using System.Windows;
using TestingSystem.ViewModels;

namespace TestingSystem.Views;

/// <summary>Code-behind только для чисто визуальных задач: фокус и запрет закрытия без разблокировки.</summary>
public partial class SessionLockWindow : Window
{
    private bool _allowClose;

    public SessionLockWindow(SessionLockViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        viewModel.CloseRequested += (_, _) =>
        {
            _allowClose = true;
            Close();
        };

        Loaded += (_, _) =>
        {
            Activate();
            PasswordInput.Focus();
        };
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        // Alt+F4 не должен обходить блокировку (но не мешаем завершению работы системы/приложения)
        if (!_allowClose && !Application.Current.Dispatcher.HasShutdownStarted) e.Cancel = true;
        base.OnClosing(e);
    }
}
