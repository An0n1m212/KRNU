using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Views;

/// <summary>Тиждень 6 (ТЗ п.3.8.2): середні бали, % складання, статистика за групами, динаміка студента.</summary>
public partial class AnalyticsWindow : Window
{
    private readonly ServiceProvider _sp;

    public AnalyticsWindow(ServiceProvider sp)
    {
        InitializeComponent();
        _sp = sp;
        var me = _sp.GetRequiredService<IAuthService>().GetCurrentUser();
        var stats = _sp.GetRequiredService<StatisticsService>();
        if (me is not null)
        {
            var t = stats.GetTeacherStats(me);
            TeacherText.Text = $"Тестів створено: {t.TestsCreated}. Перевірено: {t.WorksReviewed}. Очікують: {t.PendingReview}. Середній бал: {t.AveragePercent:0.0}%";
            GroupsList.ItemsSource = stats.GetGroupStats(me)
                .Select(g => $"{g.GroupName}: студентів {g.Students}, спроб {g.Attempts}, середній {g.AveragePercent:0.0}%, склали {g.PassRatePercent:0.0}%").ToList();
        }
        StudentBox.ItemsSource = _sp.GetRequiredService<IUserRepository>().GetStudents();
    }

    private void OnStudent(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (StudentBox.SelectedItem is not User u) return;
        var stats = _sp.GetRequiredService<StatisticsService>();
        var s = stats.GetStudentStats(u.Id);
        ProgressList.ItemsSource = stats.GetStudentProgress(u.Id)
            .Select(p => $"{p.Date:g} — {p.Percent}%").ToList();
        Title = $"Аналітика — {u.FullName}: спроб {s.TestsCompleted}, середній {s.AveragePercent:0.0}%, сертифікатів {s.Certificates}";
    }
}
