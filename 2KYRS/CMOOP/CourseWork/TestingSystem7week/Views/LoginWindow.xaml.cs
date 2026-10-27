using System.Windows;
using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Views;

/// <summary>Тиждень 2: вхід за логіном/паролем (PBKDF2), далі — маршрутизація за роллю.</summary>
public partial class LoginWindow : Window
{
    private readonly IAuthService _auth;
    private readonly IGroupRepository _groups;
    private readonly Action<User> _onSuccess;
    private readonly Action _onRegister;

    public LoginWindow(IAuthService auth, IGroupRepository groups, Action<User> onSuccess, Action onRegister)
    {
        InitializeComponent();
        _auth = auth;
        _groups = groups;
        _onSuccess = onSuccess;
        _onRegister = onRegister;
    }

    private void OnLogin(object sender, RoutedEventArgs e)
    {
        var user = _auth.Login(LoginBox.Text, PasswordBox.Password);
        if (user is null)
        {
            ErrorText.Text = "Невірний логін або пароль.";
            return;
        }
        _onSuccess(user);
        Close();
    }

    private void OnRegister(object sender, RoutedEventArgs e)
    {
        _onRegister();
        Close();
    }
}
