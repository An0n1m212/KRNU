using System.Windows.Data;
using System.Windows.Markup;
using TestingSystem.Abstractions;

namespace TestingSystem.Infrastructure;

/// <summary>
/// XAML-расширение локализации: Text="{loc:T Login_Title}".
/// Создаёт привязку к индексатору ILocalizationService, поэтому текст меняется сразу при смене языка.
/// Service задаётся один раз в Composition Root (App.OnStartup) — это единственное «статическое» место,
/// нужное только потому, что у XAML-расширений нет доступа к DI.
/// </summary>
public class TExtension : MarkupExtension
{
    public static ILocalizationService? Service { get; set; }

    public string Key { get; set; }

    public TExtension(string key) => Key = key;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (Service is null) return Key;   // режим дизайнера

        var binding = new Binding($"[{Key}]") { Source = Service, Mode = BindingMode.OneWay };
        return binding.ProvideValue(serviceProvider);
    }
}
