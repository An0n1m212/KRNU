using System.Windows;
using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Views;

/// <summary>
/// Тиждень 4: повний конструктор — 6 типів (ТЗ п.3.3.2) + усі налаштування (ТЗ п.3.3.1) + <see cref="TestValidator"/>.
/// Формати введення відповідей:
/// Single/Multiple — рядок на варіант, правильні — індекси через кому;
/// Text — рядок на допустиму відповідь;
/// Matching — рядки «ліва =&gt; права» (праві мають бути унікальними);
/// Sequence — рядки у ПРАВИЛЬНОМУ порядку (студент побачить перемішаними);
/// FileUpload — поле відповідей ігнорується (до 10 МБ, документи/зображення/архіви/код).
/// </summary>
public partial class TestEditorWindow : Window
{
    private readonly ITestRepository _tests;
    private readonly IGroupRepository _groups;
    private readonly Test _test;

    public TestEditorWindow(ITestRepository tests, IGroupRepository groups, Test test)
    {
        InitializeComponent();
        _tests = tests;
        _groups = groups;
        _test = test;
        TypeBox.ItemsSource = Enum.GetValues<QuestionType>();
        TypeBox.SelectedIndex = 0;
        TypeBox.SelectionChanged += (_, _) => UpdateHint();
        TitleBox.Text = _test.Title;
        DescBox.Text = _test.Description;
        PinBox.Text = _test.AccessCode ?? string.Empty;
        TimeBox.Text = _test.TimeLimitMinutes.ToString();
        AttemptsBox.Text = _test.MaxAttempts.ToString();
        PassBox.Text = _test.PassScorePercent.ToString();
        ShuffleQBox.IsChecked = _test.IsRandomizeQuestions;
        ShuffleABox.IsChecked = _test.IsRandomizeAnswers;
        ExplainBox.IsChecked = _test.AllowShowExplanations;
        GroupsBox.ItemsSource = _groups.GetAll();
        foreach (var g in GroupsBox.ItemsSource)
            if (_test.AssignedGroupIds.Contains(((Group)g).Id)) GroupsBox.SelectedItems.Add(g);
        UpdateHint();
        RefreshQuestions();
    }

    private void UpdateHint()
    {
        if (TypeBox.SelectedItem is not QuestionType t) return;
        AnswersHint.Text = t switch
        {
            QuestionType.SingleChoice or QuestionType.MultipleChoice => "Варіанти: кожен з нового рядка; правильні — індекси через кому.",
            QuestionType.TextAnswer => "Допустимі відповіді: кожна з нового рядка (точний збіг / містить / ручна — у фіналі).",
            QuestionType.Matching => "Пари: кожна з нового рядка у форматі «ліва => права».",
            QuestionType.Sequence => "Елементи у ПРАВИЛЬНОМУ порядку, кожен з нового рядка.",
            QuestionType.FileUpload => "Файл: відповіді не потрібні (до 10 МБ).",
            _ => string.Empty
        };
    }

    private void RefreshQuestions()
    {
        QuestionsList.ItemsSource = null;
        QuestionsList.ItemsSource = _test.Questions;
    }

    private void OnAdd(object sender, RoutedEventArgs e)
    {
        var type = TypeBox.SelectedItem is QuestionType t ? t : QuestionType.SingleChoice;
        var q = new Question { Text = "Нове питання", QuestionType = type, MaxScore = 1 };
        if (type is QuestionType.SingleChoice or QuestionType.MultipleChoice)
        {
            q.Answers.Add(new Answer { Text = "Варіант 1", IsCorrect = true, OrderIndex = 0 });
            q.Answers.Add(new Answer { Text = "Варіант 2", OrderIndex = 1 });
        }
        _test.Questions.Add(q);
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
        ExplBox.Text = q.Explanation;
        UpdateHint();
        switch (q.QuestionType)
        {
            case QuestionType.Matching:
                AnswersBox.Text = string.Join("\n", q.Answers.OrderBy(a => a.OrderIndex).Select(a => $"{a.Text} => {a.CorrectPairText}"));
                CorrectBox.Text = string.Empty;
                break;
            case QuestionType.Sequence:
                AnswersBox.Text = string.Join("\n", q.Answers.OrderBy(a => a.OrderIndex).Select(a => a.Text));
                CorrectBox.Text = string.Empty;
                break;
            case QuestionType.TextAnswer:
                AnswersBox.Text = string.Join("\n", q.Answers.Where(a => a.IsCorrect).Select(a => a.Text));
                CorrectBox.Text = string.Empty;
                break;
            case QuestionType.FileUpload:
                AnswersBox.Text = string.Empty;
                CorrectBox.Text = string.Empty;
                break;
            default:
                AnswersBox.Text = string.Join("\n", q.Answers.Select(a => a.Text));
                CorrectBox.Text = string.Join(",", q.Answers.Select((a, i) => (a, i)).Where(t => t.a.IsCorrect).Select(t => t.i));
                break;
        }
    }

    private void OnApply(object sender, RoutedEventArgs e)
    {
        if (QuestionsList.SelectedItem is not Question q) return;
        q.QuestionType = TypeBox.SelectedItem is QuestionType t ? t : q.QuestionType;
        q.Text = QTextBox.Text;
        q.MaxScore = double.TryParse(ScoreBox.Text, out var s) ? s : 1;
        q.Explanation = ExplBox.Text;
        var lines = AnswersBox.Text.Split('\n').Select(x => x.TrimEnd('\r')).Where(x => x.Trim().Length > 0).ToList();
        q.Answers.Clear();
        switch (q.QuestionType)
        {
            case QuestionType.SingleChoice:
            case QuestionType.MultipleChoice:
                var correct = CorrectBox.Text.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.TryParse(x.Trim(), out var i) ? i : -1).Where(i => i >= 0 && i < lines.Count).ToHashSet();
                for (int i = 0; i < lines.Count; i++)
                    q.Answers.Add(new Answer { Text = lines[i].Trim(), IsCorrect = correct.Contains(i), OrderIndex = i });
                break;
            case QuestionType.TextAnswer:
                for (int i = 0; i < lines.Count; i++)
                    q.Answers.Add(new Answer { Text = lines[i].Trim(), IsCorrect = true, OrderIndex = i });
                break;
            case QuestionType.Matching:
                int idx = 0;
                foreach (var line in lines)
                {
                    var parts = line.Split(new[] { "=>" }, StringSplitOptions.None);
                    if (parts.Length != 2) continue;
                    q.Answers.Add(new Answer { Text = parts[0].Trim(), CorrectPairText = parts[1].Trim(), OrderIndex = idx++ });
                }
                break;
            case QuestionType.Sequence:
                for (int i = 0; i < lines.Count; i++)
                    q.Answers.Add(new Answer { Text = lines[i].Trim(), OrderIndex = i });
                break;
            case QuestionType.FileUpload:
                break;
        }
        ErrorText.Text = TestValidator.TryValidateQuestion(q, out var err) ? string.Empty : err;
        RefreshQuestions();
    }

    private void OnSave(object sender, RoutedEventArgs e)
    {
        _test.Title = TitleBox.Text.Trim();
        _test.Description = DescBox.Text.Trim();
        _test.AccessCode = string.IsNullOrWhiteSpace(PinBox.Text) ? null : PinBox.Text.Trim();
        _test.TimeLimitMinutes = int.TryParse(TimeBox.Text, out var tl) ? Math.Max(0, tl) : 0;
        _test.MaxAttempts = int.TryParse(AttemptsBox.Text, out var ma) ? Math.Max(0, ma) : 0;
        _test.PassScorePercent = double.TryParse(PassBox.Text, out var p) ? Math.Clamp(p, 0, 100) : 60;
        _test.IsRandomizeQuestions = ShuffleQBox.IsChecked == true;
        _test.IsRandomizeAnswers = ShuffleABox.IsChecked == true;
        _test.AllowShowExplanations = ExplainBox.IsChecked != false;
        _test.AssignedGroupIds.Clear();
        _test.AssignedGroupIds.AddRange(GroupsBox.SelectedItems.OfType<Group>().Select(g => g.Id));
        if (!TestValidator.TryValidate(_test, out var err))
        {
            ErrorText.Text = err;
            return;
        }
        _tests.Save(_test);
        Close();
    }
}
