using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;
using TestingSystem.Abstractions;
using TestingSystem.Models;
using TestingSystem.Services;

namespace TestingSystem.Views;

/// <summary>Представление вопроса одного типа: умеет показать сохранённый ответ и считать новый.</summary>
public interface IQuestionView
{
    FrameworkElement Element { get; }
    void Load(StudentAnswerDetail? existing);
    StudentAnswerDetail Read();
}

/// <summary>
/// ПАТТЕРН «ФАБРИКА»: по типу вопроса создаёт нужный UI (Radio / CheckBox / TextBox / сопоставление /
/// сортировка / загрузка файла). Окно теста не знает деталей — оно работает с IQuestionView.
/// Новый тип вопроса = новый класс + одна строка в этом switch.
/// </summary>
public static class QuestionViewFactory
{
    public static IQuestionView Create(RuntimeQuestion rq, AntiCheatMonitor monitor, ILocalizationService loc)
        => rq.Source.QuestionType switch
        {
            QuestionType.SingleChoice => new ChoiceView(rq, multiple: false),
            QuestionType.MultipleChoice => new ChoiceView(rq, multiple: true),
            QuestionType.TextAnswer => new TextAnswerView(rq),
            QuestionType.Matching => new MatchingView(rq, loc),
            QuestionType.Sequence => new SequenceView(rq, loc),
            QuestionType.FileUpload => new FileUploadView(rq, monitor, loc),
            _ => throw new NotSupportedException($"Unsupported question type {rq.Source.QuestionType}")
        };
}

// ---------------------------------------------------------------- Single / Multiple
internal sealed class ChoiceView : IQuestionView
{
    private readonly StackPanel _panel = new();
    private readonly List<ToggleButton> _buttons = new();
    private readonly int _questionId;

    public FrameworkElement Element => _panel;

    public ChoiceView(RuntimeQuestion rq, bool multiple)
    {
        _questionId = rq.Source.Id;
        foreach (var answer in rq.Answers)
        {
            ToggleButton button = multiple
                ? new CheckBox()                                              // MultipleChoice: CheckBox
                : new RadioButton { GroupName = $"q{_questionId}" };          // SingleChoice: RadioButton
            button.Tag = answer.Id;
            button.Content = new TextBlock { Text = answer.Text, TextWrapping = TextWrapping.Wrap, FontSize = 16 };
            button.Margin = new Thickness(0, 5, 0, 5);
            _buttons.Add(button);
            _panel.Children.Add(button);
        }
    }

    public void Load(StudentAnswerDetail? existing)
    {
        foreach (var b in _buttons) b.IsChecked = existing?.SelectedAnswerIds.Contains((int)b.Tag) == true;
    }

    public StudentAnswerDetail Read() => new()
    {
        QuestionId = _questionId,
        SelectedAnswerIds = _buttons.Where(b => b.IsChecked == true).Select(b => (int)b.Tag).ToList()
    };
}

// ---------------------------------------------------------------- TextAnswer
internal sealed class TextAnswerView : IQuestionView
{
    private readonly TextBox _box = new()
    {
        AcceptsReturn = true, TextWrapping = TextWrapping.Wrap, MinHeight = 120, Padding = new Thickness(6), FontSize = 15,
        VerticalScrollBarVisibility = ScrollBarVisibility.Auto
    };
    private readonly int _questionId;

    public FrameworkElement Element => _box;
    public TextAnswerView(RuntimeQuestion rq) => _questionId = rq.Source.Id;

    public void Load(StudentAnswerDetail? existing) => _box.Text = existing?.GivenTextAnswer ?? string.Empty;
    public StudentAnswerDetail Read() => new() { QuestionId = _questionId, GivenTextAnswer = _box.Text };
}

// ---------------------------------------------------------------- Matching
internal sealed record RightOption(int OwnerAnswerId, string Text);

internal sealed class MatchingView : IQuestionView
{
    private readonly Grid _grid = new();
    private readonly List<(Answer Left, ComboBox Combo)> _rows = new();
    private readonly int _questionId;

    public FrameworkElement Element => _grid;

    public MatchingView(RuntimeQuestion rq, ILocalizationService loc)
    {
        _questionId = rq.Source.Id;
        _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(24) });
        _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        // Правые части перемешиваются — иначе порядок подсказывал бы ответ
        var rng = new Random();
        var options = rq.Answers.Select(a => new RightOption(a.Id, a.CorrectPairText)).OrderBy(_ => rng.Next()).ToList();

        int row = 0;
        foreach (var left in rq.Answers)       // слева — элементы в порядке OrderIndex
        {
            _grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var text = new TextBlock { Text = left.Text, FontSize = 16, TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 6, 0, 6) };
            var arrow = new TextBlock { Text = "→", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            var combo = new ComboBox { ItemsSource = options, DisplayMemberPath = "Text", Margin = new Thickness(0, 6, 0, 6), Padding = new Thickness(6), FontSize = 15 };

            Grid.SetRow(text, row); Grid.SetColumn(text, 0);
            Grid.SetRow(arrow, row); Grid.SetColumn(arrow, 1);
            Grid.SetRow(combo, row); Grid.SetColumn(combo, 2);
            _grid.Children.Add(text); _grid.Children.Add(arrow); _grid.Children.Add(combo);
            _rows.Add((left, combo));
            row++;
        }
    }

    public void Load(StudentAnswerDetail? existing)
    {
        for (int i = 0; i < _rows.Count; i++)
        {
            int id = existing != null && i < existing.SelectedAnswerIds.Count ? existing.SelectedAnswerIds[i] : 0;
            var combo = _rows[i].Combo;
            combo.SelectedItem = combo.Items.Cast<RightOption>().FirstOrDefault(o => o.OwnerAnswerId == id);
        }
    }

    /// <summary>i-й элемент = Id варианта, чья правая часть выбрана для i-го левого элемента (0 — не выбрано).</summary>
    public StudentAnswerDetail Read() => new()
    {
        QuestionId = _questionId,
        SelectedAnswerIds = _rows.Select(r => (r.Combo.SelectedItem as RightOption)?.OwnerAnswerId ?? 0).ToList()
    };
}

// ---------------------------------------------------------------- Sequence
internal sealed class SequenceView : IQuestionView
{
    private readonly DockPanel _panel = new();
    private readonly ListBox _list = new() { MinHeight = 160, FontSize = 16, DisplayMemberPath = "Text" };
    private readonly ObservableCollection<Answer> _items;
    private readonly int _questionId;

    public FrameworkElement Element => _panel;

    public SequenceView(RuntimeQuestion rq, ILocalizationService loc)
    {
        _questionId = rq.Source.Id;
        _items = new ObservableCollection<Answer>(rq.Answers);   // уже перемешаны сервисом
        _list.ItemsSource = _items;                              // привязываем один раз; дальше только Move

        var up = new Button { Content = "▲", Width = 40, Margin = new Thickness(0, 0, 0, 6), ToolTip = loc["Hint_MoveUp"] };
        var down = new Button { Content = "▼", Width = 40, ToolTip = loc["Hint_MoveDown"] };
        up.Click += (_, _) => Move(-1);
        down.Click += (_, _) => Move(+1);

        var buttons = new StackPanel { Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
        buttons.Children.Add(up);
        buttons.Children.Add(down);

        DockPanel.SetDock(buttons, Dock.Right);
        _panel.Children.Add(buttons);
        _panel.Children.Add(_list);
    }

    private void Move(int delta)
    {
        int i = _list.SelectedIndex, j = i + delta;
        if (i < 0 || j < 0 || j >= _items.Count) return;
        _items.Move(i, j);
        _list.SelectedIndex = j;
    }

    public void Load(StudentAnswerDetail? existing)
    {
        if (existing is { SelectedAnswerIds.Count: > 0 })
        {
            var byId = _items.ToDictionary(a => a.Id);
            var ordered = existing.SelectedAnswerIds.Where(byId.ContainsKey).Select(id => byId[id]).ToList();
            if (ordered.Count == _items.Count)
            {
                for (int i = 0; i < ordered.Count; i++)
                {
                    int current = _items.IndexOf(ordered[i]);
                    if (current != i) _items.Move(current, i);
                }
            }
        }
        _list.SelectedIndex = -1;
    }

    public StudentAnswerDetail Read() => new() { QuestionId = _questionId, SelectedAnswerIds = _items.Select(a => a.Id).ToList() };
}

// ---------------------------------------------------------------- FileUpload
internal sealed class FileUploadView : IQuestionView
{
    private readonly StackPanel _panel = new();
    private readonly TextBlock _fileName = new() { Margin = new Thickness(0, 8, 0, 0), FontSize = 14 };
    private readonly TextBlock _error = new() { Margin = new Thickness(0, 6, 0, 0), TextWrapping = TextWrapping.Wrap };
    private readonly AntiCheatMonitor _monitor;
    private readonly ILocalizationService _loc;
    private readonly int _questionId;
    private string? _path;

    public FrameworkElement Element => _panel;

    public FileUploadView(RuntimeQuestion rq, AntiCheatMonitor monitor, ILocalizationService loc)
    {
        _questionId = rq.Source.Id;
        _monitor = monitor;
        _loc = loc;
        _error.SetResourceReference(TextBlock.ForegroundProperty, "ErrorBrush");

        var choose = new Button { Content = loc["Btn_ChooseFile"], Padding = new Thickness(14, 6, 14, 6), HorizontalAlignment = HorizontalAlignment.Left };
        var clear = new Button { Content = loc["Btn_RemoveFile"], Padding = new Thickness(14, 6, 14, 6), Margin = new Thickness(8, 0, 0, 0) };
        choose.Click += (_, _) => Choose();
        clear.Click += (_, _) => { _path = null; UpdateLabel(); };

        var row = new StackPanel { Orientation = Orientation.Horizontal };
        row.Children.Add(choose);
        row.Children.Add(clear);
        _panel.Children.Add(row);
        _panel.Children.Add(_fileName);
        _panel.Children.Add(_error);
    }

    private void Choose()
    {
        _error.Text = "";
        var dialog = new OpenFileDialog
        {
            Filter = "Allowed files|" + string.Join(";", AppFiles.AllowedUploadExtensions.Select(e => "*" + e))
        };

        // Диалог — окно самого приложения: на время его показа античит не должен считать это нарушением
        bool? ok;
        using (_monitor.Suspend()) ok = dialog.ShowDialog();
        if (ok != true) return;

        var info = new FileInfo(dialog.FileName);
        if (!AppFiles.IsAllowedUpload(info.FullName)) { _error.Text = _loc["Err_FileType"]; return; }   // безопасность: whitelist расширений
        if (info.Length > AppFiles.MaxUploadBytes) { _error.Text = _loc["Err_FileTooBig"]; return; }

        try { _path = AppFiles.CopyIn("uploads", info.FullName, "work"); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { _error.Text = _loc["Err_FileCopy"]; return; }
        UpdateLabel();
    }

    private void UpdateLabel() => _fileName.Text = _path is null ? _loc["Lbl_NoFile"] : $"📎 {Path.GetFileName(_path)}";

    public void Load(StudentAnswerDetail? existing)
    {
        _path = existing?.UploadedFilePath;
        UpdateLabel();
    }

    public StudentAnswerDetail Read() => new() { QuestionId = _questionId, UploadedFilePath = _path };
}
