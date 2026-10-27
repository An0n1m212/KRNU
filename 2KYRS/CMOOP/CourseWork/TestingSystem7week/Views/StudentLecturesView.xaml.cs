using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;
using TestingSystem.Infrastructure;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Views;

/// <summary>Тиждень 7 (ТЗ п.3.6.2): читання опублікованих лекцій своєї групи + перехід до тесту.</summary>
public partial class StudentLecturesView : Window
{
    private readonly ServiceProvider _sp;
    private readonly TestSessionService _sessions;
    private readonly ITestRepository _tests;
    private List<Lecture> _items = new();

    public StudentLecturesView(ServiceProvider sp, TestSessionService sessions, ITestRepository tests)
    {
        InitializeComponent();
        _sp = sp;
        _sessions = sessions;
        _tests = tests;
        _items = _sp.GetRequiredService<ILectureService>().GetForCurrentStudent().ToList();
        LecturesList.ItemsSource = _items;
        if (_items.Count > 0) LecturesList.SelectedIndex = 0;
    }

    private void OnSelect(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (LecturesList.SelectedItem is Lecture l)
            Preview.Document = MarkdownRenderer.Render(l.Content);
    }

    private void OnRelated(object sender, RoutedEventArgs e)
    {
        if (LecturesList.SelectedItem is not Lecture l || l.RelatedTestId is not int testId) return;
        var me = _sp.GetRequiredService<IAuthService>().GetCurrentUser();
        if (me is null) return;
        var test = _tests.GetById(testId);
        if (test is null) return;
        string? pin = null;
        if (!string.IsNullOrWhiteSpace(test.AccessCode))
        {
            var dlg = new AccessCodeDialog { Owner = this };
            if (dlg.ShowDialog() != true) return;
            pin = dlg.AccessCode;
        }
        var (error, controller) = _sessions.TryStart(me, testId, pin);
        if (error != StartError.None || controller is null) { MessageBox.Show("Не вдалося почати: " + error); return; }
        new TestRunWindow(controller).Show();
        Close();
    }
}
