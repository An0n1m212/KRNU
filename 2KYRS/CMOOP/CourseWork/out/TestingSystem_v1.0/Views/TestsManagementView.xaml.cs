using System.Windows;
using System.Windows.Controls;
using TestingSystem.Models;

namespace TestingSystem.Views;

public record TestListItem(Test Source, string Title, string Description, string Meta);

public partial class TestsManagementView : UserControl
{
    private readonly AppServices _s;

    public TestsManagementView(AppServices services)
    {
        InitializeComponent();
        _s = services;

        Loaded += (_, _) =>
        {
            _s.Localization.LanguageChanged -= OnLanguageChanged;
            _s.Localization.LanguageChanged += OnLanguageChanged;
            Reload();
        };
        Unloaded += (_, _) => _s.Localization.LanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(object? sender, EventArgs e) => Reload();

    /// <summary>Тесты, которыми пользователь вправе управлять (учитель — свои, админ — все).</summary>
    private void Reload()
    {
        var loc = _s.Localization;
        var categories = _s.Categories.GetAll().ToDictionary(c => c.Id, c => c.Name);
        var groups = _s.Groups.GetAll().ToDictionary(g => g.Id, g => g.Name);

        TestsList.ItemsSource = _s.Tests.GetAll().Where(_s.Auth.CanManage).Select(t =>
        {
            string cat = t.CategoryId is int cid && categories.TryGetValue(cid, out var cn) ? cn : "—";
            string assigned = t.AssignedGroupIds.Count == 0
                ? loc["Meta_AllGroups"]
                : string.Join(", ", t.AssignedGroupIds.Select(id => groups.TryGetValue(id, out var gn) ? gn : "?"));

            string meta = string.Join("  •  ", new[]
            {
                $"{loc["Lbl_Category"]}: {cat}",
                t.TimeLimitMinutes > 0 ? loc.Format("Card_Time", t.TimeLimitMinutes) : loc["Card_NoLimit"],
                loc.Format("Card_Rules", t.PassScorePercent, t.Questions.Count),
                t.MaxAttempts > 0 ? loc.Format("Meta_MaxAttempts", t.MaxAttempts) : loc["Meta_UnlimitedAttempts"],
                string.IsNullOrWhiteSpace(t.AccessCode) ? "" : "🔒 " + loc["Card_Protected"],
                $"{loc["Lbl_Groups"]}: {assigned}"
            }.Where(x => x.Length > 0));

            return new TestListItem(t, t.Title, t.Description, meta);
        }).ToList();
    }

    private void Create_Click(object sender, RoutedEventArgs e) => OpenEditor(null);
    private void Edit_Click(object sender, RoutedEventArgs e) => EditSelected();
    private void TestsList_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) => EditSelected();

    private void EditSelected()
    {
        if (TestsList.SelectedItem is not TestListItem item)
        {
            MessageBox.Show(_s.Localization["Msg_SelectTest"]); return;
        }
        if (!_s.Auth.CanManage(item.Source))             // права: учитель правит только свои тесты
        {
            MessageBox.Show(_s.Localization["Err_Forbidden"]); return;
        }
        OpenEditor(_s.Tests.GetById(item.Source.Id));
    }

    private void OpenEditor(Test? existing)
    {
        var editor = new TestEditorWindow(_s, existing) { Owner = Window.GetWindow(this) };
        if (editor.ShowDialog() == true) Reload();
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        var loc = _s.Localization;
        if (TestsList.SelectedItem is not TestListItem item)
        {
            MessageBox.Show(loc["Msg_SelectTest"]); return;
        }
        if (!_s.Auth.CanManage(item.Source))
        {
            MessageBox.Show(loc["Err_Forbidden"]); return;
        }
        if (MessageBox.Show(loc.Format("Msg_ConfirmDelete", item.Title), loc["Msg_ConfirmTitle"], MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

        _s.Tests.Delete(item.Source.Id);
        Reload();
    }
}
