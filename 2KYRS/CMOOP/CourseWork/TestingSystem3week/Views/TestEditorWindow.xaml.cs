using System.Windows;
using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Views;

/// <summary>
/// Тиждень 3: конструктор, частина 1 — типи Single / Multiple / Text + <see cref="TestValidator"/>.
/// Тиждень 4 додасть Matching / Sequence / FileUpload і повні налаштування тесту.
/// </summary>
public partial class TestEditorWindow : Window
{
    private readonly ITestRepository _tests;
    private readonly ICategoryRepository _categories;
    private readonly Test _test;

    public TestEditorWindow(ITestRepository tests, ICategoryRepository categories, Test test)
    {
        InitializeComponent();
        _tests = tests;
        _categories = categories;
        _test = test;
        TypeBox.ItemsSource = new[] { QuestionType.SingleChoice, QuestionType.MultipleChoice, QuestionType.TextAnswer };
        TypeBox.SelectedIndex = 0;
        TitleBox.Text = _test.Title;
        DescBox.Text = _test.Description;
        PassBox.Text = _test.PassScorePercent.ToString();
        RefreshQuestions();
    }

    private void RefreshQuestions()
    {
        QuestionsList.ItemsSource = null;
        QuestionsList.ItemsSource = _test.Questions;
    }

    private void OnAdd(object sender, RoutedEventArgs e)
    {
        _test.Questions.Add(new Question
        {
            Text = "Нове питання",
            QuestionType = (QuestionType)TypeBox.SelectedItem,
            MaxScore = 1,
            Answers = { new Answer { Text = "Варіант 1", IsCorrect = true, OrderIndex = 0 }, new Answer { Text = "Варіант 2", OrderIndex = 1 } }
        });
        RefreshQuestions();
    }

    private void OnDelete(object sender, RoutedEventArgs e)
    {
        if (QuestionsList.SelectedItem is Question q) _test.Questions.Remove(q);
        RefreshQuestions();
    }

    private void OnSelect(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (QuestionsList.SelectedItem is not Question q) return;
        TypeBox.SelectedItem = q.QuestionType;
        QTextBox.Text = q.Text;
        ScoreBox.Text = q.MaxScore.ToString();
        AnswersBox.Text = string.Join("\n", q.Answers.Select(a => a.Text));
        CorrectBox.Text = string.Join(",", q.Answers.Select((a, i) => (a, i)).Where(t => t.a.IsCorrect).Select(t => t.i));
    }

    private void OnApply(object sender, RoutedEventArgs e)
    {
        if (QuestionsList.SelectedItem is not Question q) return;
        q.QuestionType = (QuestionType)TypeBox.SelectedItem;
        q.Text = QTextBox.Text;
        q.MaxScore = double.TryParse(ScoreBox.Text, out var s) ? s : 1;
        var lines = AnswersBox.Text.Split('\n').Select(x => x.TrimEnd('\r')).ToList();
        var correct = CorrectBox.Text.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => int.TryParse(x.Trim(), out var i) ? i : -1).Where(i => i >= 0 && i < lines.Count).ToHashSet();
        q.Answers.Clear();
        for (int i = 0; i < lines.Count; i++)
            q.Answers.Add(new Answer { Text = lines[i], IsCorrect = correct.Contains(i), OrderIndex = i });
        if (!TestValidator.TryValidateQuestion(q, out var err)) ErrorText.Text = err;
        else ErrorText.Text = string.Empty;
        RefreshQuestions();
    }

    private void OnSave(object sender, RoutedEventArgs e)
    {
        _test.Title = TitleBox.Text.Trim();
        _test.Description = DescBox.Text.Trim();
        _test.PassScorePercent = double.TryParse(PassBox.Text, out var p) ? p : 60;
        if (!TestValidator.TryValidate(_test, out var err))
        {
            ErrorText.Text = err;
            return;
        }
        _tests.Save(_test);
        Close();
    }
}
