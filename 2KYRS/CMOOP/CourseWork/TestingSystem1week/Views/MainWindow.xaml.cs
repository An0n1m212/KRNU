using System.Windows;
using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Views;

/// <summary>Тиждень 1: наочна перевірка шарів Models → Abstractions → Persistence + DI.</summary>
public partial class MainWindow : Window
{
    private readonly IUserRepository _users;
    private readonly IGroupRepository _groups;

    public MainWindow(IUserRepository users, IGroupRepository groups)
    {
        InitializeComponent();
        _users = users;
        _groups = groups;
        Refresh();
    }

    private void Refresh()
    {
        GroupsList.ItemsSource = _groups.GetAll();
        UsersList.ItemsSource = _users.GetAll();
    }

    private void OnAddGroup(object sender, RoutedEventArgs e)
    {
        var name = GroupNameBox.Text.Trim();
        if (name.Length == 0) return;
        _groups.Save(new Group { Name = name });
        GroupNameBox.Clear();
        Refresh();
    }

    private void OnRefresh(object sender, RoutedEventArgs e) => Refresh();
}
