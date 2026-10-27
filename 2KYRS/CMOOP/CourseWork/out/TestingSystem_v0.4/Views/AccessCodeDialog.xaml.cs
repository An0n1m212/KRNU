using System.Windows;

namespace TestingSystem.Views;

/// <summary>Запрос пин-кода доступа к защищённому тесту.</summary>
public partial class AccessCodeDialog : Window
{
    public string Code => CodeBox.Password;

    public AccessCodeDialog()
    {
        InitializeComponent();
        Loaded += (_, _) => CodeBox.Focus();
    }

    private void Ok_Click(object sender, RoutedEventArgs e) => DialogResult = true;
}
