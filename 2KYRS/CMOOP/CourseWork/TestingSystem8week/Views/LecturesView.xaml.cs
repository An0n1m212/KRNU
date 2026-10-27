using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Views;

/// <summary>Тиждень 7 (ТЗ п.3.6.1): CRUD лекцій, чернетка/публікація.</summary>
public partial class LecturesView : Window
{
    private readonly ServiceProvider _sp;

    public LecturesView(ServiceProvider sp)
    {
        InitializeComponent();
        _sp = sp;
        Refresh();
    }

    private ILectureService Lectures => _sp.GetRequiredService<ILectureService>();

    private void Refresh()
    {
        var items = Lectures.GetManageable();
        LecturesList.Tag = items.ToList();
        LecturesList.ItemsSource = items.Select(l => $"{(l.IsPublished ? "[ОПУБЛ] " : "[ЧЕРНЕТКА] ")}{l.Title}").ToList();
    }

    private void OnNew(object sender, RoutedEventArgs e)
    {
        var me = _sp.GetRequiredService<IAuthService>().GetCurrentUser();
        new LectureEditorWindow(_sp, new Lecture { AuthorTeacherId = me?.Id ?? 0 }) { Owner = this }.ShowDialog();
        Refresh();
    }

    private void OnEdit(object sender, RoutedEventArgs e)
    {
        if (LecturesList.SelectedIndex < 0) return;
        var items = (List<Lecture>)LecturesList.Tag;
        var copy = items[LecturesList.SelectedIndex].DeepClone();
        new LectureEditorWindow(_sp, copy) { Owner = this }.ShowDialog();
        Refresh();
    }

    private void OnToggle(object sender, RoutedEventArgs e)
    {
        if (LecturesList.SelectedIndex < 0) return;
        var items = (List<Lecture>)LecturesList.Tag;
        var l = items[LecturesList.SelectedIndex].DeepClone();
        l.IsPublished = !l.IsPublished;
        l.UpdatedAt = DateTime.Now;
        try { Lectures.Save(l); } catch (UnauthorizedAccessException) { MessageBox.Show("Немає прав."); }
        Refresh();
    }

    private void OnDelete(object sender, RoutedEventArgs e)
    {
        if (LecturesList.SelectedIndex < 0) return;
        var items = (List<Lecture>)LecturesList.Tag;
        if (MessageBox.Show("Видалити лекцію?", "Підтвердження", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
        try { Lectures.Delete(items[LecturesList.SelectedIndex].Id); } catch (UnauthorizedAccessException) { MessageBox.Show("Немає прав."); }
        Refresh();
    }

    private void OnRefresh(object sender, RoutedEventArgs e) => Refresh();
}
