using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Views;

/// <summary>
/// Тиждень 8 (ТЗ п.3.7.2): теми → лекція → тест; усе — чернеткою в редактори на перевірку.
/// Мова відповідей AI = мова інтерфейсу (ТЗ п.3.7.3).
/// </summary>
public partial class AiAssistantWindow : Window
{
    private readonly ServiceProvider _sp;
    private CancellationTokenSource? _cts;

    public AiAssistantWindow(ServiceProvider sp)
    {
        InitializeComponent();
        _sp = sp;
        ProviderText.Text = "Провайдер: " + _sp.GetRequiredService<IAiAssistant>().ProviderDescription;
    }

    private IAiAssistant Ai => _sp.GetRequiredService<IAiAssistant>();

    private void SetBusy(bool busy)
    {
        Progress.IsIndeterminate = busy;
        StatusText.Text = busy ? "Виконується…" : string.Empty;
    }

    private void OnCancel(object sender, RoutedEventArgs e) => _cts?.Cancel();

    private int Count() => int.TryParse(CountBox.Text, out var n) ? Math.Clamp(n, 1, 20) : 5;

    private async void OnTopics(object sender, RoutedEventArgs e)
    {
        _cts = new CancellationTokenSource();
        SetBusy(true);
        try
        {
            var topics = await Ai.SuggestTopicsAsync(TopicBox.Text.Trim(), Count(), _cts.Token);
            OutputList.ItemsSource = topics.Select(t => $"{t.Title} — {t.Description}").ToList();
            StatusText.Text = $"Тем: {topics.Count}.";
        }
        catch (OperationCanceledException) { StatusText.Text = "Скасовано."; }
        catch (Exception ex) { StatusText.Text = "Помилка: " + ex.Message; }
        finally { SetBusy(false); }
    }

    private async void OnLecture(object sender, RoutedEventArgs e)
    {
        _cts = new CancellationTokenSource();
        SetBusy(true);
        try
        {
            var lec = await Ai.GenerateLectureAsync(TopicBox.Text.Trim(), null, _cts.Token);
            var me = _sp.GetRequiredService<IAuthService>().GetCurrentUser();
            var draft = new Lecture
            {
                Title = lec.Title, Summary = lec.Summary, Content = lec.Content,
                AuthorTeacherId = me?.Id ?? 0, IsPublished = false, IsAiGenerated = true,
                CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now
            };
            StatusText.Text = "Лекцію-чернетку відкрито в редакторі. Перевірте і збережіть.";
            new LectureEditorWindow(_sp, draft) { Owner = this }.ShowDialog();
        }
        catch (OperationCanceledException) { StatusText.Text = "Скасовано."; }
        catch (Exception ex) { StatusText.Text = "Помилка: " + ex.Message; }
        finally { SetBusy(false); }
    }

    private async void OnTest(object sender, RoutedEventArgs e)
    {
        _cts = new CancellationTokenSource();
        SetBusy(true);
        try
        {
            var res = await Ai.GenerateTestAsync(TopicBox.Text.Trim(), null, Count(),
                Enum.GetValues<QuestionType>(), _cts.Token);
            var me = _sp.GetRequiredService<IAuthService>().GetCurrentUser();
            res.Test.AuthorTeacherId = me?.Id ?? 0;
            StatusText.Text = res.SkippedQuestions > 0
                ? $"Тест-чернетку відкрито. Битих питань відкинуто: {res.SkippedQuestions}."
                : "Тест-чернетку відкрито в редакторі. Перевірте і збережіть.";
            OutputList.ItemsSource = res.Test.Questions.Select(q => $"{q.QuestionType}: {q.Text}").ToList();
            new TestEditorWindow(
                _sp.GetRequiredService<ITestRepository>(),
                _sp.GetRequiredService<IGroupRepository>(), res.Test) { Owner = this }.ShowDialog();
        }
        catch (OperationCanceledException) { StatusText.Text = "Скасовано."; }
        catch (Exception ex) { StatusText.Text = "Помилка: " + ex.Message; }
        finally { SetBusy(false); }
    }
}
