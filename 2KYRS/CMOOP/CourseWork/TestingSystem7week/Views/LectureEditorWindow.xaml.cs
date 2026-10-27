using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;
using TestingSystem.Infrastructure;
using TestingSystem.Models;

namespace TestingSystem.Views;

/// <summary>Тиждень 7 (ТЗ п.3.6.1): Markdown-редактор, чернетка/публікація, прив'язка тесту.</summary>
public partial class LectureEditorWindow : Window
{
    private readonly ServiceProvider _sp;
    private readonly Lecture _lecture;

    public LectureEditorWindow(ServiceProvider sp, Lecture lecture)
    {
        InitializeComponent();
        _sp = sp;
        _lecture = lecture;
        TitleBox.Text = lecture.Title;
        SummaryBox.Text = lecture.Summary;
        ContentBox.Text = lecture.Content;
        PublishBox.IsChecked = lecture.IsPublished;
        var tests = _sp.GetRequiredService<ITestRepository>().GetAll().ToList();
        tests.Insert(0, new Test { Title = "— без тесту —" });
        TestBox.ItemsSource = tests;
        TestBox.SelectedIndex = lecture.RelatedTestId is int id
            ? Math.Max(0, tests.FindIndex(t => t.Id == id)) : 0;
        GroupsBox.ItemsSource = _sp.GetRequiredService<IGroupRepository>().GetAll();
        foreach (var g in GroupsBox.ItemsSource)
            if (lecture.AssignedGroupIds.Contains(((Group)g).Id)) GroupsBox.SelectedItems.Add(g);
        Preview.Document = MarkdownRenderer.Render(ContentBox.Text);
    }

    private void OnText(object sender, System.Windows.Controls.TextChangedEventArgs e) =>
        Preview.Document = MarkdownRenderer.Render(ContentBox.Text);

    private void OnSave(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleBox.Text)) { ErrorText.Text = "Назва порожня."; return; }
        _lecture.Title = TitleBox.Text.Trim();
        _lecture.Summary = SummaryBox.Text.Trim();
        _lecture.Content = ContentBox.Text;
        _lecture.IsPublished = PublishBox.IsChecked == true;
        _lecture.RelatedTestId = TestBox.SelectedItem is Test t && t.Id != 0 ? t.Id : null;
        _lecture.AssignedGroupIds.Clear();
        _lecture.AssignedGroupIds.AddRange(GroupsBox.SelectedItems.OfType<Group>().Select(g => g.Id));
        _lecture.UpdatedAt = DateTime.Now;
        if (_lecture.CreatedAt == default) _lecture.CreatedAt = DateTime.Now;
        try
        {
            _sp.GetRequiredService<ILectureService>().Save(_lecture);
            Close();
        }
        catch (UnauthorizedAccessException) { ErrorText.Text = "Немає прав."; }
    }
}
