using System.Windows;
using System.Windows.Controls;

namespace TestingSystem.Views;

/// <summary>Верхняя панель: пользователь (аватар, ФИО, роль), язык, тема, индикатор подключения, выход.</summary>
public partial class HeaderBar : UserControl
{
    private AppServices? _services;

    public event EventHandler? ProfileRequested;
    public event EventHandler? LogoutRequested;

    public HeaderBar()
    {
        InitializeComponent();
        Loaded += (_, _) => Subscribe();
        Unloaded += (_, _) => Unsubscribe();
    }

    public void Init(AppServices services)
    {
        _services = services;
        Settings.Init(services.Localization, services.Theme);
        if (IsLoaded) Subscribe();
    }

    private void Subscribe()
    {
        if (_services is null) return;
        Unsubscribe();
        _services.Auth.CurrentUserChanged += OnChanged;
        _services.Localization.LanguageChanged += OnChanged;
        Refresh();
    }

    private void Unsubscribe()
    {
        if (_services is null) return;
        _services.Auth.CurrentUserChanged -= OnChanged;
        _services.Localization.LanguageChanged -= OnChanged;
    }

    private void OnChanged(object? sender, EventArgs e) => Refresh();

    private void Refresh()
    {
        if (_services is null) return;
        ConnectionText.Text = _services.Localization.Format("Hdr_Connected", _services.Storage.Name);

        var user = _services.Auth.GetCurrentUser();
        if (user is null) return;
        NameText.Text = user.FullName;
        RoleText.Text = _services.Localization[$"Role_{user.Role}"];
        Avatar.Show(user.Id, user.FullName, user.AvatarPath);
    }

    private void Profile_Click(object sender, RoutedEventArgs e) => ProfileRequested?.Invoke(this, EventArgs.Empty);
    private void Logout_Click(object sender, RoutedEventArgs e) => LogoutRequested?.Invoke(this, EventArgs.Empty);
}
