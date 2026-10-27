using System.Windows;
using System.Windows.Controls;
using TestingSystem.Abstractions;

namespace TestingSystem.Services;

/// <summary>
/// Светлая/тёмная тема. Два слоя:
///  1) Application.ThemeMode (встроенная Fluent-тема WPF, .NET 9+) перекрашивает стандартные контролы;
///  2) словарь Themes/{Light|Dark}Colors.xaml даёт приложению свои кисти (карточки, вторичный текст, ошибки...).
/// Экраны используют эти кисти через DynamicResource, поэтому переключение действует сразу.
/// </summary>
public class ThemeService : IThemeService
{
    private ResourceDictionary? _palette;

    public AppTheme CurrentTheme { get; private set; } = AppTheme.Light;
    public event EventHandler? ThemeChanged;

    public void SetTheme(AppTheme theme)
    {
        var app = Application.Current;

        var palette = new ResourceDictionary
        {
            Source = new Uri($"pack://application:,,,/Themes/{theme}Colors.xaml", UriKind.Absolute)
        };
        if (_palette != null) app.Resources.MergedDictionaries.Remove(_palette);
        app.Resources.MergedDictionaries.Add(palette);
        _palette = palette;

        var mode = theme == AppTheme.Dark ? ThemeMode.Dark : ThemeMode.Light;
        app.ThemeMode = mode;
        foreach (Window w in app.Windows) w.ThemeMode = mode;   // заголовок окна и уже открытые окна

        FixListBoxItemStyle(app);

        CurrentTheme = theme;
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Стиль ListBoxItem из Fluent-темы привязывает HorizontalContentAlignment/VerticalContentAlignment к родительскому
    /// ItemsControl; при замене содержимого списка это даёт безвредные сообщения «Не удаётся найти источник: RelativeSource
    /// FindAncestor» в окне «Ошибки привязки XAML». Явно заданные значения заменяют привязку, а остальное оформление
    /// берётся из темы (BasedOn). Если стиль темы не найден — ничего не меняем.
    /// </summary>
    private static void FixListBoxItemStyle(Application app)
    {
        app.Resources.Remove(typeof(ListBoxItem));                       // убираем свой прошлый стиль (иначе цепочка BasedOn растёт)
        if (app.TryFindResource(typeof(ListBoxItem)) is not Style themeStyle) return;

        var style = new Style(typeof(ListBoxItem), themeStyle);
        style.Setters.Add(new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));
        style.Setters.Add(new Setter(Control.VerticalContentAlignmentProperty, VerticalAlignment.Center));
        app.Resources[typeof(ListBoxItem)] = style;
    }
}
