using System.Threading;
using TestingSystem.Abstractions;
using TestingSystem.Infrastructure.Mvvm;
using TestingSystem.Services.Ai;

namespace TestingSystem.ViewModels;

/// <summary>Пара «значення — підпис» для списків вибору.</summary>
public sealed record Option<T>(T Value, string Text);

/// <summary>View повідомляє ViewModel, коли вона з'явилась/зникла (оновлення даних, підписки на події).</summary>
public interface IViewLifecycle
{
    void OnShown();
    void OnHidden();
}

/// <summary>Елемент зі «галочкою» для списків множинного вибору (наприклад, групи).</summary>
public sealed class SelectableItem<T> : ViewModelBase
{
    private bool _isSelected;

    public T Item { get; }
    public string Name { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public SelectableItem(T item, string name, bool isSelected)
    {
        Item = item;
        Name = name;
        _isSelected = isSelected;
    }
}

/// <summary>
/// Базовий клас ViewModel із довгими операціями (виклики AI): індикатор IsBusy, скасування, повідомлення про помилку.
/// Усі виключення перетворюються на локалізований текст — View не бачить стек-трейсів.
/// </summary>
public abstract class BusyViewModelBase : ViewModelBase
{
    private bool _isBusy;
    private string _errorMessage = string.Empty;
    private string _statusMessage = string.Empty;
    private CancellationTokenSource? _cts;

    protected ILocalizationService Loc { get; }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        protected set => SetProperty(ref _errorMessage, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        protected set => SetProperty(ref _statusMessage, value);
    }

    public RelayCommand CancelOperationCommand { get; }

    protected BusyViewModelBase(ILocalizationService loc)
    {
        Loc = loc;
        CancelOperationCommand = new RelayCommand(() => _cts?.Cancel(), () => IsBusy);
    }

    protected async Task RunBusyAsync(Func<CancellationToken, Task> action)
    {
        if (IsBusy) return;

        ErrorMessage = string.Empty;
        StatusMessage = string.Empty;
        IsBusy = true;
        _cts = new CancellationTokenSource();
        try
        {
            await action(_cts.Token);
        }
        catch (OperationCanceledException)
        {
            StatusMessage = Loc["Ai_Cancelled"];
        }
        catch (Exception ex)
        {
            ErrorMessage = AiErrorFormatter.Describe(Loc, ex);
        }
        finally
        {
            IsBusy = false;
            _cts.Dispose();
            _cts = null;
        }
    }
}
