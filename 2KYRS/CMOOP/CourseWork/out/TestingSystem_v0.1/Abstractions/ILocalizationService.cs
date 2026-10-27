using System.ComponentModel;

namespace TestingSystem.Abstractions;

public enum AppLanguage { UA, RU, EN }

/// <summary>Локализация «на лету»: индексатор + PropertyChanged("Item[]") обновляют XAML-привязки {loc:T Key}.</summary>
public interface ILocalizationService : INotifyPropertyChanged
{
    AppLanguage CurrentLanguage { get; }
    void SetLanguage(AppLanguage language);
    string this[string key] { get; }
    string Format(string key, params object[] args);
    event EventHandler? LanguageChanged;
}
