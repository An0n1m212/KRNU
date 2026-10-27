using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Views;

/// <summary>
/// Тиждень 6 (ТЗ п.3.5.1): файл відповіді відкривається, бал у межах максимуму, примітка,
/// завершення переводить роботу в «завершено». AI-підказки — тиждень 8.
/// </summary>
public partial class ReviewWindow : Window
{
    private readonly ServiceProvider _sp;
    private readonly Result _result;
    private readonly Test? _test;

    public ReviewWindow(ServiceProvider sp, int resultId)
    {
        InitializeComponent();
        _sp = sp;
        _result = sp.GetRequiredService<IResultRepository>().GetById(resultId)!;
        _test = sp.GetRequiredService<ITestRepository>().GetById(_result.TestId);
        var student = sp.GetRequiredService<IUserRepository>().GetById(_result.StudentId);
        HeadText.Text = $"{student?.FullName} — {_test?.Title} ({_result.Date:g})";
        CommentBox.Text = _result.TeacherComment ?? string.Empty;
        Refresh();
    }

    private void Refresh()
    {
        DetailsList.ItemsSource = null;
        DetailsList.ItemsSource = _result.Details.Select((d, i) =>
            $"#{i + 1} {Short(d.QuestionText)} — {d.ScoreAwarded}/{d.MaxScore}" + (d.IsGraded ? "" : " (потребує оцінки)")).ToList();
    }

    private static string Short(string s) => s.Length <= 60 ? s : s[..60] + "…";

    private void OnSelect(object sender, SelectionChangedEventArgs e)
    {
        if (DetailsList.SelectedIndex < 0) return;
        var d = _result.Details[DetailsList.SelectedIndex];
        var panel = new StackPanel { Margin = new Thickness(10) };
        panel.Children.Add(new TextBlock { Text = d.QuestionText, TextWrapping = TextWrapping.Wrap });
        panel.Children.Add(new TextBlock { Text = $"Відповідь студента: {DetailText(d)}", TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 6, 0, 0) });
        if (!string.IsNullOrWhiteSpace(d.UploadedFilePath))
        {
            var open = new Button { Content = $"Відкрити файл: {System.IO.Path.GetFileName(d.UploadedFilePath)}", Margin = new Thickness(0, 6, 0, 0) };
            string path = d.UploadedFilePath;
            open.Click += (_, _) => { try { Process.Start(new ProcessStartInfo(path) { UseShellExecute = true }); } catch { } };
            panel.Children.Add(open);
        }
        panel.Children.Add(new TextBlock { Text = $"Бал (0–{d.MaxScore}):", Margin = new Thickness(0, 8, 0, 0) });
        var scoreBox = new TextBox { Text = d.ScoreAwarded.ToString() };
        panel.Children.Add(scoreBox);
        panel.Children.Add(new TextBlock { Text = "Примітка:", Margin = new Thickness(0, 6, 0, 0) });
        var noteBox = new TextBox { Text = d.TeacherNote ?? string.Empty };
        panel.Children.Add(noteBox);
        var save = new Button { Content = "Зберегти бал", Width = 130, Margin = new Thickness(0, 8, 0, 0) };
        save.Click += (_, _) =>
        {
            double score = double.TryParse(scoreBox.Text, out var s) ? s : 0;
            _sp.GetRequiredService<GradingEngine>().ApplyManualScore(d, score, noteBox.Text);
            _sp.GetRequiredService<IResultRepository>().SaveResult(_result);
            Refresh();
            (save.Parent as StackPanel)?.Children.Add(new TextBlock { Text = "Збережено." });
        };
        panel.Children.Add(save);
        var dlg = new Window { Title = "Оцінювання відповіді", Content = new ScrollViewer { Content = panel }, Width = 520, Height = 420, Owner = this };
        dlg.ShowDialog();
    }

    private static string DetailText(StudentAnswerDetail d) =>
        !string.IsNullOrWhiteSpace(d.GivenTextAnswer) ? d.GivenTextAnswer
        : !string.IsNullOrWhiteSpace(d.GivenSummary) ? d.GivenSummary
        : string.IsNullOrWhiteSpace(d.UploadedFilePath) ? "(порожньо)" : System.IO.Path.GetFileName(d.UploadedFilePath);

    private void OnFinish(object sender, RoutedEventArgs e)
    {
        var me = _sp.GetRequiredService<IAuthService>().GetCurrentUser();
        if (_result.Details.Any(d => !d.IsGraded))
        {
            MessageBox.Show("Не всі відповіді оцінено.");
            return;
        }
        _sp.GetRequiredService<TestSessionService>().CompleteReview(_result, me?.Id ?? 0, CommentBox.Text);
        MessageBox.Show("Роботу завершено.");
        Close();
    }
}
