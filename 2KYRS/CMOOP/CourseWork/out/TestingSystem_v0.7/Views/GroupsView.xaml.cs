using System.Windows;
using System.Windows.Controls;
using TestingSystem.Models;

namespace TestingSystem.Views;

/// <summary>Строка таблицы групп: название и число студентов в ней.</summary>
public sealed class GroupRow
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Students { get; init; }
}

public partial class GroupsView : UserControl
{
    private readonly AppServices _s;

    public GroupsView(AppServices services)
    {
        InitializeComponent();
        _s = services;

        if (!_s.Auth.IsInRole(Role.Admin))
            throw new InvalidOperationException("Access denied: Admin role required.");

        Loaded += (_, _) =>
        {
            _s.Localization.LanguageChanged -= OnLanguageChanged;
            _s.Localization.LanguageChanged += OnLanguageChanged;
            Reload();
        };
        Unloaded += (_, _) => _s.Localization.LanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(object? sender, EventArgs e) => Reload();

    private void Reload()
    {
        var studentsByGroup = _s.Users.GetAll()
            .Where(u => u.GroupId.HasValue)
            .GroupBy(u => u.GroupId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        GroupsGrid.ItemsSource = _s.Groups.GetAll()
            .Select(g => new GroupRow
            {
                Id = g.Id,
                Name = g.Name,
                Students = studentsByGroup.TryGetValue(g.Id, out int n) ? n : 0
            }).ToList();

        HideError();
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        var loc = _s.Localization;
        string name = NameBox.Text.Trim();

        if (name.Length == 0) { ShowError(loc["Err_GroupNameRequired"]); return; }
        if (_s.Groups.GetAll().Any(g => string.Equals(g.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            ShowError(loc["Err_GroupNameTaken"]);
            return;
        }

        _s.Groups.Save(new Group { Name = name });
        NameBox.Clear();
        Reload();
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        var loc = _s.Localization;
        if (GroupsGrid.SelectedItem is not GroupRow row) { ShowError(loc["Msg_SelectGroup"]); return; }

        if (_s.Users.GetAll().Any(u => u.GroupId == row.Id)) { ShowError(loc["Err_GroupHasStudents"]); return; }

        var answer = MessageBox.Show(
            loc.Format("Msg_ConfirmDeleteGroup", row.Name),
            loc["Msg_ConfirmTitle"], MessageBoxButton.YesNo, MessageBoxImage.Question,
            MessageBoxResult.No);
        if (answer != MessageBoxResult.Yes) return;

        _s.Groups.Delete(row.Id);
        Reload();
    }

    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorText.Visibility = Visibility.Visible;
    }

    private void HideError() => ErrorText.Visibility = Visibility.Collapsed;
}
