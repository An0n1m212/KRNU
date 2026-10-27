using System.Windows;

namespace TestingSystem.Infrastructure;

public static class WindowHelper
{
    /// <summary>Активне видиме вікно застосунку (для Owner діалогів та MessageBox).</summary>
    public static Window? ActiveWindow()
    {
        var windows = Application.Current.Windows.OfType<Window>().Where(w => w.IsVisible).ToList();
        return windows.FirstOrDefault(w => w.IsActive) ?? windows.LastOrDefault();
    }
}
