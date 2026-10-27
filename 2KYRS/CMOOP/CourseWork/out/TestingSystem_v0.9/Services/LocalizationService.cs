using System.ComponentModel;
using TestingSystem.Abstractions;

namespace TestingSystem.Services;

/// <summary>
/// Локализация на основе таблицы строк (см. Strings.cs). При смене языка поднимает
/// PropertyChanged("Item[]") — все привязки вида {loc:T Key} обновляются мгновенно.
/// Источник строк легко заменить на .resx/JSON, не меняя интерфейс.
/// </summary>
public class LocalizationService : ILocalizationService
{
    public AppLanguage CurrentLanguage { get; private set; } = AppLanguage.UA;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? LanguageChanged;

    public string this[string key]
    {
        get
        {
            if (!Strings.Table.TryGetValue(key, out var t)) return $"[{key}]";   // видно, что ключа нет
            return CurrentLanguage switch { AppLanguage.UA => t.Ua, AppLanguage.RU => t.Ru, _ => t.En };
        }
    }

    public string Format(string key, params object[] args) => string.Format(this[key], args);

    public void SetLanguage(AppLanguage language)
    {
        if (language == CurrentLanguage) return;
        CurrentLanguage = language;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));   // обновить все {loc:T ...}
        LanguageChanged?.Invoke(this, EventArgs.Empty);                          // обновить тексты, собранные в коде
    }
}

public static class LocalizationExtensions
{
    /// <summary>Локализованное описание результата операции: ключ = "Err_" + имя значения AuthResult.</summary>
    public static string Describe(this ILocalizationService loc, AuthResult result)
        => loc.Format($"Err_{result}", AuthService.MinPasswordLength);
}
