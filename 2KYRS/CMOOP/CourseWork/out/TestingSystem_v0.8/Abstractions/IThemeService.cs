namespace TestingSystem.Abstractions;

public enum AppTheme { Light, Dark }

public interface IThemeService
{
    AppTheme CurrentTheme { get; }
    void SetTheme(AppTheme theme);
    event EventHandler? ThemeChanged;
}
