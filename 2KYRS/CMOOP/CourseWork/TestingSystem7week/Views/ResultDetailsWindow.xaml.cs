using System.Windows;
using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Views;

/// <summary>Тиждень 6 (ТЗ п.3.8.3): розбір з правильними відповідями, якщо викладач дозволив.</summary>
public partial class ResultDetailsWindow : Window
{
    public ResultDetailsWindow(ITestRepository tests, Result result)
    {
        InitializeComponent();
        var test = tests.GetById(result.TestId);
        HeadText.Text = $"{test?.Title}: {result.TotalScore}/{result.MaxScore} ({result.Percentage}%)";
        bool show = test?.AllowShowExplanations != false;
        DetailsList.ItemsSource = result.Details.Select((d, i) =>
            $"#{i + 1} {d.QuestionText}\n" +
            $"Ваша відповідь: {Given(d)} — {d.ScoreAwarded}/{d.MaxScore}\n" +
            (show ? $"Правильно: {d.CorrectSummary}\nПояснення: {d.Explanation}" : "(розбір приховано викладачем)")).ToList();
    }

    private static string Given(StudentAnswerDetail d) =>
        !string.IsNullOrWhiteSpace(d.GivenTextAnswer) ? d.GivenTextAnswer
        : !string.IsNullOrWhiteSpace(d.GivenSummary) ? d.GivenSummary : "(порожньо)";
}
