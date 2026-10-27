using TestingSystem.Models;

namespace TestingSystem.Abstractions;

/// <summary>ViewModel діалогового вікна: сам закривається подією (не знає про Window).</summary>
public interface IDialogViewModel
{
    string Title { get; }
    event EventHandler<bool?>? CloseRequested;
}

/// <summary>Діалоги для ViewModel без залежності від WPF (MessageBox, модальні вікна).</summary>
public interface IDialogService
{
    void ShowMessage(string message);
    bool Confirm(string message);

    /// <summary>Показує модальне вікно, зареєстроване для типу ViewModel. Результат — DialogResult.</summary>
    bool? ShowDialog(IDialogViewModel viewModel);
}

/// <summary>
/// Міст до редактора тестів (він поки що на code-behind, не на MVVM): ViewModel просить відкрити
/// редактор з чернеткою, не знаючи про конкретне вікно.
/// </summary>
public interface ITestEditorLauncher
{
    bool? ShowTestEditor(Test? test);
}

/// <summary>Створює ViewModel із залежностями з DI + явними параметрами (наприклад, лекцією для редагування).</summary>
public interface IViewModelFactory
{
    T Create<T>(params object[] args) where T : class;
}
