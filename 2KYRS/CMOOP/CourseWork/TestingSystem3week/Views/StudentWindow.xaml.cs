using System.Windows;
using TestingSystem.Abstractions;

namespace TestingSystem.Views;

public partial class StudentWindow : Window
{
    private readonly Action _onLogout;

    public StudentWindow(ITestRepository tests, Action onLogout)
    {
        InitializeComponent();
        _onLogout = onLogout;
        TestsList.ItemsSource = tests.GetAll();
    }

    private void OnLogout(object sender, RoutedEventArgs e)
    {
        _onLogout();
        Close();
    }
}
