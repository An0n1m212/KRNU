using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;

namespace TestingSystem.Views;

/// <summary>
/// Тиждень 7 (ТЗ п.3.9): лише адміністратор; перевірка порожньої назви та дублікату;
/// групу зі студентами видаляти заборонено.
/// </summary>
public partial class GroupsView : Window
{
    private readonly ServiceProvider _sp;

    public GroupsView(ServiceProvider sp)
    {
        InitializeComponent();
        _sp = sp;
        Refresh();
    }

    private void Refresh()
    {
        var groups = _sp.GetRequiredService<IGroupRepository>().GetAll();
        var students = _sp.GetRequiredService<IUserRepository>().GetStudents();
        GroupsList.ItemsSource = groups.Select(g => $"{g.Name} — студентів: {students.Count(u => u.GroupId == g.Id)}").ToList();
        GroupsList.Tag = groups.ToList();
    }

    private void OnAdd(object sender, RoutedEventArgs e)
    {
        var name = NameBox.Text.Trim();
        if (name.Length == 0) { MessageBox.Show("Назва порожня."); return; }
        var groups = _sp.GetRequiredService<IGroupRepository>();
        if (groups.GetAll().Any(g => string.Equals(g.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show("Така група вже є.");
            return;
        }
        groups.Save(new Models.Group { Name = name });
        NameBox.Clear();
        Refresh();
    }

    private void OnDelete(object sender, RoutedEventArgs e)
    {
        if (GroupsList.SelectedIndex < 0) return;
        var groups = (List<Models.Group>)GroupsList.Tag;
        var g = groups[GroupsList.SelectedIndex];
        int students = _sp.GetRequiredService<IUserRepository>().GetStudents().Count(u => u.GroupId == g.Id);
        if (students > 0)
        {
            MessageBox.Show($"Групу зі студентами ({students}) видаляти заборонено. Переведіть студентів до інших груп.");
            return;
        }
        if (MessageBox.Show($"Видалити «{g.Name}»?", "Підтвердження", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
        _sp.GetRequiredService<IGroupRepository>().Delete(g.Id);
        Refresh();
    }
}
