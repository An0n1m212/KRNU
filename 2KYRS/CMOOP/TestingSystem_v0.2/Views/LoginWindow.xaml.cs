using System.Windows;
using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Views;

public partial class LoginWindow : Window
{
    private readonly AppServices _services;
    private readonly Action<User> _onLoggedIn;   // маршрутизация по ролям (App.ShowWorkspace)
    private bool _registerMode;

    public LoginWindow(AppServices services, Action<User> onLoggedIn)
    {
        InitializeComponent();
        _services = services;
        _onLoggedIn = onLoggedIn;

        Settings.Init(services.Localization, services.Theme);
        GroupCombo.ItemsSource = services.Groups.GetAll();   // список учебных групп для регистрации

        services.Localization.LanguageChanged += OnLanguageChanged;
        Closed += (_, _) => services.Localization.LanguageChanged -= OnLanguageChanged;

        ApplyMode();
        UsernameBox.Focus();
    }

    private void OnLanguageChanged(object? sender, EventArgs e) => ApplyMode();

    private void SwitchMode_Click(object sender, RoutedEventArgs e)
    {
        _registerMode = !_registerMode;
        ApplyMode();
    }

    private void ApplyMode()
    {
        var loc = _services.Localization;
        var v = _registerMode ? Visibility.Visible : Visibility.Collapsed;
        FullNamePanel.Visibility = v;
        ExtraPanel.Visibility = v;

        ModeTitle.Text = loc[_registerMode ? "Register_Title" : "Login_Title"];
        SubmitButton.Content = loc[_registerMode ? "Btn_Register" : "Btn_Login"];
        SwitchModeButton.Content = loc[_registerMode ? "Link_ToLogin" : "Link_ToRegister"];
        ErrorText.Text = "";
    }

    private void Submit_Click(object sender, RoutedEventArgs e)
    {
        var loc = _services.Localization;
        string username = UsernameBox.Text.Trim();
        string password = PasswordInput.Password;

        if (username.Length == 0 || password.Length == 0)
        {
            ErrorText.Text = loc["Msg_EnterCredentials"]; return;
        }

        if (_registerMode)
        {
            var group = GroupCombo.SelectedItem as Group;
            var reg = _services.Auth.RegisterStudent(username, password, FullNameBox.Text, EmailBox.Text, group?.Id);
            if (reg != AuthResult.Success)
            {
                ErrorText.Text = loc.Describe(reg); return;
            }
        }

        var user = _services.Auth.Login(username, password);
        if (user == null)
        {
            ErrorText.Text = loc["Err_InvalidCredentials"]; return;
        }

        _onLoggedIn(user);   // сначала открываем кабинет,
        Close();             // затем закрываем окно входа
    }
}
