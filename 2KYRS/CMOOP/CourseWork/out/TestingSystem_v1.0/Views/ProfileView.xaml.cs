using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Views;

/// <summary>
/// Личный кабинет (общий для всех ролей): карточка профиля, аватар, смена пароля, системные настройки
/// и статистика, зависящая от роли (ученик: динамика и сертификаты; учитель: проверки и группы).
/// </summary>
public partial class ProfileView : UserControl
{
    private readonly AppServices _s;
    private string? _avatarPath;             // аватар, который будет сохранён
    private string? _pendingAvatarSource;    // выбранный, но ещё не скопированный файл

    public ProfileView(AppServices services)
    {
        InitializeComponent();
        _s = services;
        Settings.Init(services.Localization, services.Theme);

        Loaded += (_, _) =>
        {
            _s.Localization.LanguageChanged -= OnLanguageChanged;
            _s.Localization.LanguageChanged += OnLanguageChanged;
            Reload();
        };
        Unloaded += (_, _) => _s.Localization.LanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        ProfileStatus.Text = "";
        PasswordStatus.Text = "";
        var user = _s.Auth.GetCurrentUser();
        if (user != null) ApplyTexts(user);
    }

    // ---------------------------------------------------------------- загрузка

    private void Reload()
    {
        var user = _s.Auth.GetCurrentUser();
        if (user is null) return;

        FullNameBox.Text = user.FullName;
        EmailBox.Text = user.Email;
        UsernameText.Text = user.Username;
        _avatarPath = user.AvatarPath;
        _pendingAvatarSource = null;
        Avatar.Show(user.Id, user.FullName, _avatarPath);

        OldPasswordBox.Clear(); NewPasswordBox.Clear(); ConfirmPasswordBox.Clear();
        ProfileStatus.Text = ""; PasswordStatus.Text = "";

        ApplyTexts(user);
        LoadStats(user);
    }

    private void ApplyTexts(User user)
    {
        var loc = _s.Localization;
        RoleText.Text = loc[$"Role_{user.Role}"];
        RegisteredText.Text = $"{loc["Prof_Registered"]}: {user.RegistrationDate:dd.MM.yyyy}";

        string group = user.GroupId is int id ? _s.Groups.GetById(id)?.Name ?? "—" : "";
        GroupText.Text = user.Role == Role.Student ? $"{loc["Field_Group"]}: {group}" : "";
    }

    private void LoadStats(User user)
    {
        bool student = user.Role == Role.Student;
        StudentPanel.Visibility = student ? Visibility.Visible : Visibility.Collapsed;
        TeacherPanel.Visibility = student ? Visibility.Collapsed : Visibility.Visible;

        if (student)
        {
            var s = _s.Stats.GetStudentStats(user.Id);
            SCompleted.Text = s.TestsCompleted.ToString();
            SAverage.Text = $"{s.AveragePercent:0.#}%";
            SCertificates.Text = s.Certificates.ToString();

            Chart.SetData(_s.Stats.GetStudentProgress(user.Id));

            var passed = _s.Results.GetByStudent(user.Id).Where(r => r.IsPassed && r.Status != ResultStatus.PendingTeacherReview);
            CertGrid.ItemsSource = new ResultRowFactory(_s).Build(passed);
        }
        else
        {
            var t = _s.Stats.GetTeacherStats(user);
            TCreated.Text = t.TestsCreated.ToString();
            TReviewed.Text = t.WorksReviewed.ToString();
            TPending.Text = t.PendingReview.ToString();
            TAverage.Text = $"{t.AveragePercent:0.#}%";
            GroupGrid.ItemsSource = _s.Stats.GetGroupStats(user);
        }
    }

    // ---------------------------------------------------------------- аватар

    private void ChoosePhoto_Click(object sender, RoutedEventArgs e)
    {
        var user = _s.Auth.GetCurrentUser()!;
        var dialog = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp;*.gif" };
        if (dialog.ShowDialog() != true) return;

        _pendingAvatarSource = dialog.FileName;                       // предпросмотр до сохранения
        Avatar.Show(user.Id, FullNameBox.Text, _pendingAvatarSource);
    }

    private void RemovePhoto_Click(object sender, RoutedEventArgs e)
    {
        var user = _s.Auth.GetCurrentUser()!;
        _avatarPath = null;
        _pendingAvatarSource = null;
        Avatar.Show(user.Id, FullNameBox.Text, null);                 // аватар по умолчанию
    }

    // ---------------------------------------------------------------- сохранение профиля

    private void SaveProfile_Click(object sender, RoutedEventArgs e)
    {
        var loc = _s.Localization;
        var user = _s.Auth.GetCurrentUser()!;

        string? avatarToSave = _avatarPath;
        if (_pendingAvatarSource != null)
        {
            try { avatarToSave = AppFiles.CopyIn("avatars", _pendingAvatarSource, $"user{user.Id}"); }
            catch (Exception ex) when (ex is System.IO.IOException or UnauthorizedAccessException)
            {
                SetStatus(ProfileStatus, loc["Err_AvatarCopy"], isError: true);
                return;
            }
        }

        // IAuthService -> IUserRepository.UpdateProfile: InMemory меняет запись в словаре,
        // SQLite/EF Core выполнит UPDATE Users ... WHERE Id=@id (UI и сервис не меняются)
        var result = _s.Auth.UpdateProfile(FullNameBox.Text, EmailBox.Text, avatarToSave);
        if (result != AuthResult.Success)
        {
            SetStatus(ProfileStatus, loc.Describe(result), isError: true);
            return;
        }

        Reload();   // шапка обновится по событию CurrentUserChanged
        SetStatus(ProfileStatus, loc["Msg_ProfileSaved"], isError: false);
    }

    // ---------------------------------------------------------------- смена пароля

    private void ChangePassword_Click(object sender, RoutedEventArgs e)
    {
        var loc = _s.Localization;

        // Ошибки (неверный старый пароль, слишком короткий новый, несовпадение, тот же пароль)
        // приходят кодом AuthResult и показываются на выбранном языке
        var result = _s.Auth.ChangePassword(OldPasswordBox.Password, NewPasswordBox.Password, ConfirmPasswordBox.Password);
        if (result != AuthResult.Success)
        {
            SetStatus(PasswordStatus, loc.Describe(result), isError: true);
            return;
        }

        OldPasswordBox.Clear(); NewPasswordBox.Clear(); ConfirmPasswordBox.Clear();
        SetStatus(PasswordStatus, loc["Msg_PasswordChanged"], isError: false);
    }

    // ---------------------------------------------------------------- сертификаты

    private void Certificate_Click(object sender, RoutedEventArgs e)
    {
        if (CertGrid.SelectedItem is not ResultRow row)
        {
            MessageBox.Show(_s.Localization["Msg_SelectResult"]); return;
        }
        CertificateHelper.Download(_s, row.Source, Window.GetWindow(this));
    }

    private static void SetStatus(TextBlock target, string text, bool isError)
    {
        target.Text = text;
        target.SetResourceReference(TextBlock.ForegroundProperty, isError ? "ErrorBrush" : "SuccessBrush");
    }
}
