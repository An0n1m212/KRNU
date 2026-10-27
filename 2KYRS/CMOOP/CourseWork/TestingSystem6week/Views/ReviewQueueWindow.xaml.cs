using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Views;

/// <summary>Тиждень 6 (ТЗ п.3.5.1): черга робіт «на перевірці».</summary>
public partial class ReviewQueueWindow : Window
{
    private readonly ServiceProvider _sp;

    public ReviewQueueWindow(ServiceProvider sp)
    {
        InitializeComponent();
        _sp = sp;
        Refresh();
    }

    private void Refresh()
    {
        var results = _sp.GetRequiredService<IResultRepository>().GetAll()
            .Where(r => r.Status == ResultStatus.PendingTeacherReview).OrderBy(r => r.Date).ToList();
        var users = _sp.GetRequiredService<IUserRepository>();
        var tests = _sp.GetRequiredService<ITestRepository>();
        QueueList.Tag = results;
        QueueList.ItemsSource = results.Select(r =>
            $"{r.Date:g} — {users.GetById(r.StudentId)?.FullName} — {tests.GetById(r.TestId)?.Title}").ToList();
    }

    private void OnOpen(object sender, RoutedEventArgs e)
    {
        if (QueueList.SelectedIndex < 0) return;
        var results = (List<Result>)QueueList.Tag;
        new ReviewWindow(_sp, results[QueueList.SelectedIndex].Id).ShowDialog();
        Refresh();
    }

    private void OnRefresh(object sender, RoutedEventArgs e) => Refresh();
}
