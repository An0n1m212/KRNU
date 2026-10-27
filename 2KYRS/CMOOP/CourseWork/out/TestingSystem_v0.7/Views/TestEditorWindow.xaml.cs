using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Views;

/// <summary>
/// Конструктор теста: настройки (категория, лимит времени, проходной балл, попытки, перемешивание,
/// пин-код, группы) и банк вопросов 6 типов с медиафайлом и пояснением. Работает с копией теста —
/// в хранилище пишет только по «Сохранить тест».
/// </summary>
public partial class TestEditorWindow : Window
{
    private const int Slots = 8;

    private sealed class Slot
    {
        public RadioButton Radio = null!;
        public CheckBox Check = null!;
        public TextBox Text = null!;
        public TextBox Pair = null!;
    }

    private readonly AppServices _s;
    private readonly Test _test;
    private readonly ObservableCollection<Question> _questions;
    private readonly List<Slot> _slots = new();
    private string? _mediaPath;
    private bool _loading;

    public TestEditorWindow(AppServices services, Test? existing)
    {
        InitializeComponent();
        _s = services;
        var loc = _s.Localization;

        _test = existing ?? new Test { AuthorTeacherId = _s.Auth.GetCurrentUser()!.Id };
        Title = existing == null ? loc["Editor_NewTest"] : loc.Format("Editor_EditTest", existing.Title);

        // --- настройки теста ---
        TitleBox.Text = _test.Title;
        DescBox.Text = _test.Description;
        TimeBox.Text = _test.TimeLimitMinutes.ToString();
        PassBox.Text = _test.PassScorePercent.ToString("0.##", CultureInfo.InvariantCulture);
        AttemptsBox.Text = _test.MaxAttempts.ToString();
        RandomQuestionsCheck.IsChecked = _test.IsRandomizeQuestions;
        RandomAnswersCheck.IsChecked = _test.IsRandomizeAnswers;
        ExplanationsCheck.IsChecked = _test.AllowShowExplanations;
        AccessCodeBox.Text = _test.AccessCode ?? "";

        var categories = _s.Categories.GetAll();
        CategoryCombo.ItemsSource = categories;
        CategoryCombo.SelectedItem = categories.FirstOrDefault(c => c.Id == _test.CategoryId);

        var groups = _s.Groups.GetAll();
        GroupsList.ItemsSource = groups;
        foreach (Group g in GroupsList.Items)
            if (_test.AssignedGroupIds.Contains(g.Id)) GroupsList.SelectedItems.Add(g);

        // --- вопросы ---
        _questions = new ObservableCollection<Question>(_test.Questions);
        QuestionsList.ItemsSource = _questions;

        QTypeCombo.ItemsSource = Enum.GetValues<QuestionType>().Select(v => new EnumItem<QuestionType>(v, loc[$"QType_{v}"])).ToList();
        TextEvalCombo.ItemsSource = Enum.GetValues<TextEvaluationType>().Select(v => new EnumItem<TextEvaluationType>(v, loc[$"TEval_{v}"])).ToList();
        QTypeCombo.SelectedIndex = 0;
        TextEvalCombo.SelectedIndex = 0;

        BuildSlots();
        ApplyType();
    }

    // ---------------------------------------------------------------- динамические поля ответов

    private void BuildSlots()
    {
        for (int i = 0; i < Slots; i++)
        {
            var grid = new Grid { Margin = new Thickness(0, 3, 0, 3) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var flags = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
            var radio = new RadioButton { GroupName = "SingleCorrect", Margin = new Thickness(0, 0, 8, 0) };
            var check = new CheckBox { Margin = new Thickness(0, 0, 8, 0) };
            flags.Children.Add(radio);
            flags.Children.Add(check);

            var text = new TextBox { Padding = new Thickness(4) };
            var pair = new TextBox { Padding = new Thickness(4), Margin = new Thickness(6, 0, 0, 0) };

            Grid.SetColumn(flags, 0); Grid.SetColumn(text, 1); Grid.SetColumn(pair, 2);
            grid.Children.Add(flags); grid.Children.Add(text); grid.Children.Add(pair);
            AnswersPanel.Children.Add(grid);
            _slots.Add(new Slot { Radio = radio, Check = check, Text = text, Pair = pair });
        }
    }

    private QuestionType SelectedType => (QTypeCombo.SelectedItem as EnumItem<QuestionType>)?.Value ?? QuestionType.SingleChoice;
    private TextEvaluationType SelectedEval => (TextEvalCombo.SelectedItem as EnumItem<TextEvaluationType>)?.Value ?? TextEvaluationType.AutoExactMatch;

    private void TypeChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_loading && IsLoaded) ApplyType();
    }

    /// <summary>Подстраивает редактор ответов под выбранный тип вопроса.</summary>
    private void ApplyType()
    {
        if (_slots.Count == 0) return;
        var type = SelectedType;
        bool manualText = type == QuestionType.TextAnswer && SelectedEval == TextEvaluationType.ManualTeacherReview;
        bool hasAnswers = type != QuestionType.FileUpload && !manualText;

        TextEvalPanel.Visibility = type == QuestionType.TextAnswer ? Visibility.Visible : Visibility.Collapsed;
        AnswersHeader.Visibility = AnswersPanel.Visibility = hasAnswers ? Visibility.Visible : Visibility.Collapsed;
        AnswersHeader.Text = _s.Localization[$"AnsHint_{type}"];

        foreach (var slot in _slots)
        {
            slot.Radio.Visibility = type == QuestionType.SingleChoice ? Visibility.Visible : Visibility.Collapsed;
            slot.Check.Visibility = type == QuestionType.MultipleChoice ? Visibility.Visible : Visibility.Collapsed;
            slot.Pair.Visibility = type == QuestionType.Matching ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    // ---------------------------------------------------------------- вопрос <-> форма

    private void QuestionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (QuestionsList.SelectedItem is Question q) LoadQuestion(q);
    }

    private void LoadQuestion(Question q)
    {
        _loading = true;
        QTypeCombo.SelectedItem = QTypeCombo.Items.Cast<EnumItem<QuestionType>>().First(i => i.Value == q.QuestionType);
        TextEvalCombo.SelectedItem = TextEvalCombo.Items.Cast<EnumItem<TextEvaluationType>>().First(i => i.Value == q.TextEvaluationType);
        _loading = false;
        ApplyType();

        QTextBox.Text = q.Text;
        MaxScoreBox.Text = q.MaxScore.ToString("0.##", CultureInfo.InvariantCulture);
        ExplanationBox.Text = q.Explanation;
        _mediaPath = q.MediaPath;
        MediaBox.Text = _mediaPath is null ? "" : Path.GetFileName(_mediaPath);

        var ordered = q.Answers.OrderBy(a => a.OrderIndex).ToList();
        for (int i = 0; i < Slots; i++)
        {
            var slot = _slots[i];
            var a = i < ordered.Count ? ordered[i] : null;
            slot.Text.Text = a?.Text ?? "";
            slot.Pair.Text = a?.CorrectPairText ?? "";
            slot.Radio.IsChecked = a?.IsCorrect == true && q.QuestionType == QuestionType.SingleChoice;
            slot.Check.IsChecked = a?.IsCorrect == true && q.QuestionType == QuestionType.MultipleChoice;
        }
    }

    private void ClearForm()
    {
        QuestionsList.SelectedItem = null;
        QTextBox.Clear();
        ExplanationBox.Clear();
        MaxScoreBox.Text = "1";
        _mediaPath = null;
        MediaBox.Clear();
        foreach (var s in _slots)
        {
            s.Text.Clear(); s.Pair.Clear();
            s.Radio.IsChecked = false; s.Check.IsChecked = false;
        }
    }

    private static bool TryParseDouble(string text, out double value)
        => double.TryParse(text.Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out value);

    /// <summary>Собирает вопрос из формы; для каждого типа — своя валидация. null — данные некорректны.</summary>
    private Question? BuildQuestion()
    {
        var loc = _s.Localization;
        var type = SelectedType;
        var eval = type == QuestionType.TextAnswer ? SelectedEval : TextEvaluationType.AutoExactMatch;

        if (string.IsNullOrWhiteSpace(QTextBox.Text)) return Fail("Err_EnterQuestion");
        if (!TryParseDouble(MaxScoreBox.Text, out double max) || max <= 0 || max > 1000) return Fail("Err_Q_Score");

        bool manualText = type == QuestionType.TextAnswer && eval == TextEvaluationType.ManualTeacherReview;
        var answers = new List<Answer>();

        if (type != QuestionType.FileUpload && !manualText)
        {
            var filled = _slots.Where(s => !string.IsNullOrWhiteSpace(s.Text.Text)).ToList();

            switch (type)
            {
                case QuestionType.SingleChoice:
                    if (filled.Count < 2) return Fail("Err_MinTwoAnswers");
                    if (filled.Count(s => s.Radio.IsChecked == true) != 1) return Fail("Err_Q_OneCorrect");
                    break;
                case QuestionType.MultipleChoice:
                    if (filled.Count < 2) return Fail("Err_MinTwoAnswers");
                    if (!filled.Any(s => s.Check.IsChecked == true)) return Fail("Err_MarkCorrect");
                    break;
                case QuestionType.TextAnswer:
                    if (filled.Count < 1) return Fail("Err_Q_NeedAccepted");
                    break;
                case QuestionType.Matching:
                    if (filled.Count < 2 || filled.Any(s => string.IsNullOrWhiteSpace(s.Pair.Text))) return Fail("Err_Q_Pairs");
                    // правые части должны быть уникальны: именно по ним ученик выбирает соответствие
                    if (filled.Select(s => s.Pair.Text.Trim().ToLowerInvariant()).Distinct().Count() != filled.Count) return Fail("Err_Q_PairsUnique");
                    break;
                case QuestionType.Sequence:
                    if (filled.Count < 2) return Fail("Err_MinTwoAnswers");
                    break;
            }

            for (int i = 0; i < filled.Count; i++)
            {
                var s = filled[i];
                answers.Add(new Answer
                {
                    Text = s.Text.Text.Trim(),
                    CorrectPairText = type == QuestionType.Matching ? s.Pair.Text.Trim() : "",
                    OrderIndex = i,
                    IsCorrect = type switch
                    {
                        QuestionType.SingleChoice => s.Radio.IsChecked == true,
                        QuestionType.MultipleChoice => s.Check.IsChecked == true,
                        QuestionType.TextAnswer => true,       // каждая запись — допустимый ответ
                        _ => false
                    }
                });
            }
        }

        return new Question
        {
            Text = QTextBox.Text.Trim(),
            QuestionType = type,
            TextEvaluationType = eval,
            MaxScore = max,
            Explanation = ExplanationBox.Text.Trim(),
            MediaPath = _mediaPath,
            Answers = answers
        };

        Question? Fail(string key)
        {
            MessageBox.Show(this, loc[key]);
            return null;
        }
    }

    // ---------------------------------------------------------------- кнопки вопросов

    private void AddQuestion_Click(object sender, RoutedEventArgs e)
    {
        var q = BuildQuestion();
        if (q == null) return;
        _questions.Add(q);
        ClearForm();
    }

    private void UpdateQuestion_Click(object sender, RoutedEventArgs e)
    {
        int idx = QuestionsList.SelectedIndex;
        if (idx < 0) { MessageBox.Show(this, _s.Localization["Err_SelectQuestionFirst"]); return; }

        var q = BuildQuestion();
        if (q == null) return;

        q.Id = _questions[idx].Id;                // сохраняем идентичность вопроса
        _questions[idx] = q;
        QuestionsList.SelectedIndex = idx;
    }

    private void RemoveQuestion_Click(object sender, RoutedEventArgs e)
    {
        int idx = QuestionsList.SelectedIndex;
        if (idx < 0) { MessageBox.Show(this, _s.Localization["Err_SelectQuestionFirst"]); return; }
        _questions.RemoveAt(idx);
        ClearForm();
    }

    private void ClearForm_Click(object sender, RoutedEventArgs e) => ClearForm();

    private void BrowseMedia_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp;*.gif" };
        if (dialog.ShowDialog(this) != true) return;
        try
        {
            _mediaPath = AppFiles.CopyIn("media", dialog.FileName, "media");   // копия, независимая от исходного файла
            MediaBox.Text = Path.GetFileName(_mediaPath);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show(this, _s.Localization["Err_FileCopy"]);
        }
    }

    private void ClearMedia_Click(object sender, RoutedEventArgs e)
    {
        _mediaPath = null;
        MediaBox.Clear();
    }

    // ---------------------------------------------------------------- сохранение теста

    private int? ResolveCategory()
    {
        string name = CategoryCombo.Text.Trim();
        if (name.Length == 0) return null;

        // Можно выбрать существующую категорию или ввести новую — она будет создана
        var existing = _s.Categories.GetAll().FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
        return (existing ?? _s.Categories.Save(new Category { Name = name })).Id;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        var loc = _s.Localization;

        if (_test.Id != 0 && !_s.Auth.CanManage(_test)) { MessageBox.Show(this, loc["Err_Forbidden"]); return; }
        if (string.IsNullOrWhiteSpace(TitleBox.Text)) { MessageBox.Show(this, loc["Err_EnterTitle"]); return; }

        if (!int.TryParse(TimeBox.Text.Trim(), out int time) || time < 0 || time > 600) { MessageBox.Show(this, loc["Err_T_Time"]); return; }
        if (!TryParseDouble(PassBox.Text, out double pass) || pass < 0 || pass > 100) { MessageBox.Show(this, loc["Err_T_Pass"]); return; }
        if (!int.TryParse(AttemptsBox.Text.Trim(), out int attempts) || attempts < 0 || attempts > 100) { MessageBox.Show(this, loc["Err_T_Attempts"]); return; }
        if (_questions.Count == 0) { MessageBox.Show(this, loc["Err_NeedQuestion"]); return; }

        int? categoryId = ResolveCategory();

        _test.Title = TitleBox.Text.Trim();
        _test.Description = DescBox.Text.Trim();
        _test.CategoryId = categoryId;
        _test.TimeLimitMinutes = time;
        _test.PassScorePercent = pass;
        _test.MaxAttempts = attempts;
        _test.IsRandomizeQuestions = RandomQuestionsCheck.IsChecked == true;
        _test.IsRandomizeAnswers = RandomAnswersCheck.IsChecked == true;
        _test.AllowShowExplanations = ExplanationsCheck.IsChecked == true;
        _test.AccessCode = string.IsNullOrWhiteSpace(AccessCodeBox.Text) ? null : AccessCodeBox.Text.Trim();
        _test.AssignedGroupIds = GroupsList.SelectedItems.Cast<Group>().Select(g => g.Id).ToList();
        _test.Questions = _questions.Select(q => { q.CategoryId ??= categoryId; return q; }).ToList();

        _s.Tests.Save(_test);
        DialogResult = true;
    }
}
