using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Views;

/// <summary>
/// Тиждень 5 (ТЗ п.3.4): навігація, лічильник відповідей, таймер з попередженням,
/// підтвердження здачі, античит (ТЗ п.3.4.3): втрата фокуса/згортання — порушення,
/// після 5 порушень — автоздача. Попередник фінального TestRunWindow.
/// </summary>
public partial class TestRunWindow : Window
{
    private const int MaxViolations = 5;
    private readonly TestSessionController _c;
    private readonly AntiCheatMonitor _anti;
    private int _index;
    private List<int> _seqOrder = new();
    private string? _filePath;

    public TestRunWindow(TestSessionController controller)
    {
        InitializeComponent();
        _c = controller;
        TitleText.Text = _c.Test.Title;
        _c.Begin();
        _c.Tick += (_, _) => UpdateTimer();
        _c.TimeWarning += (_, _) => MessageBox.Show("Залишилась менше хвилини!");
        _c.Finished += OnFinishedAuto;

        _anti = new AntiCheatMonitor(this, MaxViolations);
        _anti.ViolationRegistered += (_, e) =>
        {
            _c.RegisterViolation();
            ViolationsText.Text = $"Порушення: {e.Count}/{MaxViolations}";
            BannerText.Text = e.Kind == ViolationKind.Minimized
                ? $"Не згортайте вікно! Порушення {e.Count} з {MaxViolations}."
                : $"Не втрачайте фокус! Порушення {e.Count} з {MaxViolations}.";
            BannerText.Visibility = Visibility.Visible;
        };
        _anti.LimitReached += (_, _) =>
        {
            MessageBox.Show("Перевищено число порушень. Тест здається автоматично.");
            DoSubmit(timedOut: false);
        };

        NavList.ItemsSource = _c.Questions.Select((q, i) => $"#{i + 1} {Short(q.Source.Text)}").ToList();
        NavList.SelectedIndex = 0;
        UpdateTimer();
        UpdateCounts();
        Closed += (_, _) => _anti.Dispose();
    }

    private static string Short(string s) => s.Length <= 28 ? s : s[..28] + "…";

    private void UpdateTimer()
    {
        TimerText.Text = _c.Remaining is { } left ? $"Час: {left:mm\\:ss}" : "Без ліміту часу";
        if (_c.Remaining == TimeSpan.Zero && !_c.IsFinished) DoSubmit(timedOut: true);
    }

    private void UpdateCounts()
    {
        AnsweredText.Text = $"Відповідей: {_c.AnsweredCount}/{_c.Questions.Count}";
        ViolationsText.Text = $"Порушення: {_c.Session.ViolationsCount}/{MaxViolations}";
    }

    private void OnNav(object sender, SelectionChangedEventArgs e)
    {
        if (NavList.SelectedIndex < 0) return;
        SaveCurrent();
        _index = NavList.SelectedIndex;
        _c.SetCurrentIndex(_index);
        Render();
        UpdateCounts();
    }

    private void OnPrev(object sender, RoutedEventArgs e)
    {
        if (_index > 0) NavList.SelectedIndex = _index - 1;
    }

    private void OnNext(object sender, RoutedEventArgs e)
    {
        if (_index < _c.Questions.Count - 1) NavList.SelectedIndex = _index + 1;
    }

    private void Render()
    {
        QuestionPanel.Children.Clear();
        var rq = _c.Questions[_index];
        var q = rq.Source;
        var saved = _c.GetAnswer(q.Id);
        QuestionPanel.Children.Add(new TextBlock { Text = $"#{_index + 1}. {q.Text}  (макс. {q.MaxScore})", TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 10) });

        switch (q.QuestionType)
        {
            case QuestionType.SingleChoice:
                foreach (var a in rq.Answers)
                {
                    var rb = new RadioButton { Content = a.Text, Tag = a.Id, Margin = new Thickness(0, 2, 0, 2) };
                    if (saved?.SelectedAnswerIds.Contains(a.Id) == true) rb.IsChecked = true;
                    QuestionPanel.Children.Add(rb);
                }
                break;
            case QuestionType.MultipleChoice:
                foreach (var a in rq.Answers)
                {
                    var cb = new CheckBox { Content = a.Text, Tag = a.Id, Margin = new Thickness(0, 2, 0, 2) };
                    if (saved?.SelectedAnswerIds.Contains(a.Id) == true) cb.IsChecked = true;
                    QuestionPanel.Children.Add(cb);
                }
                break;
            case QuestionType.TextAnswer:
                var tb = new TextBox { Text = saved?.GivenTextAnswer ?? string.Empty, AcceptsReturn = true, Height = 80 };
                QuestionPanel.Children.Add(tb);
                break;
            case QuestionType.Matching:
                var rights = rq.Answers.Select(a => a.CorrectPairText).OrderBy(x => x).ToList();
                foreach (var left in rq.Answers.OrderBy(a => a.OrderIndex))
                {
                    QuestionPanel.Children.Add(new TextBlock { Text = left.Text, Margin = new Thickness(0, 6, 0, 2) });
                    var combo = new ComboBox { Tag = left.Id, ItemsSource = rights, Margin = new Thickness(0, 0, 0, 2) };
                    if (saved is not null)
                    {
                        int li = rq.Answers.OrderBy(a => a.OrderIndex).ToList().FindIndex(a => a.Id == left.Id);
                        if (li >= 0 && li < saved.SelectedAnswerIds.Count)
                        {
                            var chosenId = saved.SelectedAnswerIds[li];
                            var chosen = rq.Answers.FirstOrDefault(a => a.Id == chosenId);
                            if (chosen is not null) combo.SelectedItem = chosen.CorrectPairText;
                        }
                    }
                    QuestionPanel.Children.Add(combo);
                }
                break;
            case QuestionType.Sequence:
                _seqOrder = saved is { SelectedAnswerIds.Count: > 0 }
                    ? new List<int>(saved.SelectedAnswerIds)
                    : rq.Answers.Select(a => a.Id).ToList();
                var lb = new ListBox { Height = 160 };
                RefreshSeqList(lb);
                QuestionPanel.Children.Add(lb);
                var row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 6, 0, 0) };
                var up = new Button { Content = "Вгору", Width = 90 };
                var down = new Button { Content = "Вниз", Width = 90, Margin = new Thickness(8, 0, 0, 0) };
                up.Click += (_, _) => MoveSeq(lb, -1);
                down.Click += (_, _) => MoveSeq(lb, +1);
                row.Children.Add(up);
                row.Children.Add(down);
                QuestionPanel.Children.Add(row);
                break;
            case QuestionType.FileUpload:
                _filePath = saved?.UploadedFilePath;
                var fileLabel = new TextBlock { Text = string.IsNullOrWhiteSpace(_filePath) ? "Файл не обрано (до 10 МБ)" : $"Обрано: {Path.GetFileName(_filePath)}" };
                var btn = new Button { Content = "Обрати файл…", Width = 130, Margin = new Thickness(0, 6, 0, 0) };
                btn.Click += (_, _) =>
                {
                    using (_anti.Suspend())
                    {
                        var dlg = new OpenFileDialog();
                        if (dlg.ShowDialog() != true) return;
                        var fi = new FileInfo(dlg.FileName);
                        if (fi.Length > 10 * 1024 * 1024) { MessageBox.Show("Файл завеликий (максимум 10 МБ)."); return; }
                        if (!AppFiles.IsAllowedUpload(dlg.FileName)) { MessageBox.Show("Недозволений тип файлу."); return; }
                        _filePath = AppFiles.CopyIn("uploads", dlg.FileName);
                        fileLabel.Text = $"Обрано: {Path.GetFileName(_filePath)}";
                    }
                };
                QuestionPanel.Children.Add(fileLabel);
                QuestionPanel.Children.Add(btn);
                break;
        }
    }

    private string TextOf(int id) => _c.Questions[_index].Answers.FirstOrDefault(a => a.Id == id)?.Text ?? "?";

    private void RefreshSeqList(ListBox lb)
    {
        lb.ItemsSource = null;
        lb.ItemsSource = _seqOrder.Select(TextOf).ToList();
    }

    private void MoveSeq(ListBox lb, int dir)
    {
        int i = lb.SelectedIndex;
        int j = i + dir;
        if (i < 0 || j < 0 || j >= _seqOrder.Count) return;
        (_seqOrder[i], _seqOrder[j]) = (_seqOrder[j], _seqOrder[i]);
        RefreshSeqList(lb);
        lb.SelectedIndex = j;
    }

    private void SaveCurrent()
    {
        var rq = _c.Questions[_index];
        var q = rq.Source;
        var d = new StudentAnswerDetail { QuestionId = q.Id };
        int child = 1; // 0 — заголовок
        switch (q.QuestionType)
        {
            case QuestionType.SingleChoice:
                foreach (RadioButton rb in QuestionPanel.Children.OfType<RadioButton>())
                    if (rb.IsChecked == true) d.SelectedAnswerIds.Add((int)rb.Tag);
                break;
            case QuestionType.MultipleChoice:
                foreach (CheckBox cb in QuestionPanel.Children.OfType<CheckBox>())
                    if (cb.IsChecked == true) d.SelectedAnswerIds.Add((int)cb.Tag);
                break;
            case QuestionType.TextAnswer:
                d.GivenTextAnswer = (QuestionPanel.Children.OfType<TextBox>().FirstOrDefault()?.Text ?? string.Empty);
                break;
            case QuestionType.Matching:
                var lefts = rq.Answers.OrderBy(a => a.OrderIndex).ToList();
                var combos = QuestionPanel.Children.OfType<ComboBox>().ToList();
                for (int i = 0; i < lefts.Count && i < combos.Count; i++)
                {
                    string? right = combos[i].SelectedItem as string;
                    int owner = right is null ? 0 : rq.Answers.FirstOrDefault(a => a.CorrectPairText == right)?.Id ?? 0;
                    d.SelectedAnswerIds.Add(owner);
                }
                break;
            case QuestionType.Sequence:
                d.SelectedAnswerIds.AddRange(_seqOrder);
                break;
            case QuestionType.FileUpload:
                d.UploadedFilePath = _filePath;
                break;
        }
        _c.SaveAnswer(d);
    }

    private void OnSubmit(object sender, RoutedEventArgs e)
    {
        SaveCurrent();
        int unanswered = _c.Questions.Count - _c.AnsweredCount;
        using (_anti.Suspend())
        {
            var res = MessageBox.Show(unanswered > 0
                ? $"Без відповіді: {unanswered}. Здати?"
                : "Здати тест?", "Підтвердження", MessageBoxButton.YesNo);
            if (res != MessageBoxResult.Yes) return;
        }
        DoSubmit(timedOut: false);
    }

    private void DoSubmit(bool timedOut)
    {
        if (_c.IsFinished) return;
        SaveCurrent();
        Result result = timedOut ? _c.Expire() : _c.Submit();
        using (_anti.Suspend())
        {
            new ResultWindow(result) { Owner = this }.ShowDialog();
        }
        Close();
    }

    private void OnFinishedAuto(object? sender, Result e)
    {
        // Таймаут з TestSessionController: вікно вже закривається через DoSubmit.
    }
}
