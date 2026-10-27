using System.Windows;

namespace TestingSystem.Views;

/// <summary>Тиждень 5: діалог пін-коду (ТЗ п.3.4.1). Попередник фінального AccessCodeDialog.</summary>
public partial class AccessCodeDialog : Window
{
    public string AccessCode { get; private set; } = string.Empty;

    public AccessCodeDialog() => InitializeComponent();

    private void OnOk(object sender, RoutedEventArgs e)
    {
        AccessCode = PinBox.Password;
        DialogResult = true;
    }

    private void OnCancel(object sender, RoutedEventArgs e) => DialogResult = false;
}
