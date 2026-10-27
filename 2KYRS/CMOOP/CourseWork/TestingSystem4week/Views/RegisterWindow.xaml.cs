using System.Windows;
using TestingSystem.Abstractions;

namespace TestingSystem.Views;

/// <summary>Тиждень 2: самореєстрація лише з роллю «Студент» (ТЗ п.3.2.1).</summary>
public partial class RegisterWindow : Window
{
    private readonly IAuthService _auth;
    private readonly Action _onBack;

    public RegisterWindow(IAuthService auth, IGroupRepository groups, Action onBack)
    {
        InitializeComponent();
        _auth = auth;
        _onBack = onBack;
        GroupBox.ItemsSource = groups.GetAll();
    }

    private void OnRegister(object sender, RoutedEventArgs e)
    {
        int? groupId = (GroupBox.SelectedItem as Models.Group)?.Id;
        var result = _auth.RegisterStudent(LoginBox.Text, PasswordBox.Password, FullNameBox.Text, EmailBox.Text, groupId);
        if (result == AuthResult.Success)
        {
            MessageBox.Show("Зареєстровано. Увійдіть.");
            _onBack();
            Close();
        }
        else
        {
            ErrorText.Text = result.ToString();
        }
    }

    private void OnBack(object sender, RoutedEventArgs e)
    {
        _onBack();
        Close();
    }
}
