using System.Windows.Input;
using TestingSystem.Abstractions;

namespace TestingSystem.Infrastructure;

/// <summary>
/// Фиксирует ввод пользователя во ВСЕХ окнах приложения через InputManager (мышь, клавиатура, касания, перо).
/// Живёт всё время работы приложения, поэтому отписка не требуется. Создавать нужно в UI-потоке.
/// </summary>
public sealed class WpfInputActivitySource : IActivitySource
{
    public event EventHandler? ActivityDetected;

    public WpfInputActivitySource() => InputManager.Current.PreProcessInput += OnPreProcessInput;

    private void OnPreProcessInput(object sender, PreProcessInputEventArgs e)
    {
        if (e.StagingItem.Input is MouseEventArgs or KeyboardEventArgs or TouchEventArgs or StylusEventArgs)
            ActivityDetected?.Invoke(this, EventArgs.Empty);
    }
}
