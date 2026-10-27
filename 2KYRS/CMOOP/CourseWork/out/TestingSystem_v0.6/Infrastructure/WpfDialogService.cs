using System.Windows;
using TestingSystem.Abstractions;

namespace TestingSystem.Infrastructure;

/// <summary>
/// IDialogService для WPF. ViewModel не знає про вікна: сервіс за ТИПОМ ViewModel підбирає зареєстроване вікно,
/// задає DataContext і закриває його, коли ViewModel викликає CloseRequested.
/// Реєстрація (App.xaml.cs): .Register&lt;LectureEditorViewModel, LectureEditorWindow&gt;()
/// </summary>
public sealed class WpfDialogService : IDialogService
{
    private readonly Dictionary<Type, Func<Window>> _windows = new();

    public WpfDialogService Register<TViewModel, TWindow>()
        where TViewModel : IDialogViewModel
        where TWindow : Window, new()
    {
        _windows[typeof(TViewModel)] = () => new TWindow();
        return this;
    }

    public void ShowMessage(string message)
    {
        var owner = WindowHelper.ActiveWindow();
        if (owner != null) MessageBox.Show(owner, message); else MessageBox.Show(message);
    }

    public bool Confirm(string message)
    {
        var owner = WindowHelper.ActiveWindow();
        var result = owner != null
            ? MessageBox.Show(owner, message, string.Empty, MessageBoxButton.YesNo)
            : MessageBox.Show(message, string.Empty, MessageBoxButton.YesNo);
        return result == MessageBoxResult.Yes;
    }

    public bool? ShowDialog(IDialogViewModel viewModel)
    {
        var type = viewModel.GetType();
        if (!_windows.TryGetValue(type, out var factory))
            throw new InvalidOperationException($"No window is registered for {type.Name}.");

        var window = factory();
        window.DataContext = viewModel;
        window.Title = viewModel.Title;
        window.Owner = WindowHelper.ActiveWindow();

        EventHandler<bool?>? handler = null;
        handler = (_, result) =>
        {
            viewModel.CloseRequested -= handler;
            if (result.HasValue) window.DialogResult = result;   // закриває модальне вікно
            else window.Close();
        };
        viewModel.CloseRequested += handler;

        return window.ShowDialog();
    }
}
