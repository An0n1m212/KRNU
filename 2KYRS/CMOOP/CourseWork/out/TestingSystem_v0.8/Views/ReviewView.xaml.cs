using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TestingSystem.Infrastructure.Mvvm;
using TestingSystem.Models;
using TestingSystem.Services;
using TestingSystem.Services.Ai;

namespace TestingSystem.Views;

/// <summary>
/// Елемент ручної перевірки (одна «відкрита» відповідь учня). ScoreText/Note сповіщають UI про зміни,
/// тому підказка AI з'являється в полях одразу, а викладач може її виправити.
/// </summary>
public class ReviewItem : ViewModelBase
{
    private string _scoreText = "0";
    private string _note = "";

    public StudentAnswerDetail Detail { get; init; } = null!;
    public string QuestionText { get; init; } = "";
    public string AnswerText { get; init; } = "";
    public string FilePath { get; init; } = "";
    public string FileLabel { get; init; } = "";
    public string Reference { get; init; } = "";
    public string MaxText { get; init; } = "";
    public Visibility TextVis { get; init; }
    public Visibility FileVis { get; init; }
    public Visibility ReferenceVis { get; init; }

    public string ScoreText { get => _scoreText; set => SetProperty(ref _scoreText, value ?? ""); }
    public string Note { get => _note; set => SetProperty(ref _note, value ?? ""); }
}

/// <summary>Проверка работ со статусом PendingTeacherReview: просмотр ответов/файлов, выставление баллов и комментариев.</summary>
public partial class ReviewView : UserControl
{
    private readonly AppServices _s;
    private Result? _current;
    private List<ReviewItem> _items = new();

    public ReviewView(AppServices services)
    {
        InitializeComponent();
        _s = services;

        Loaded += (_, _) =>
        {
            _s.Localization.LanguageChanged -= OnLanguageChanged;
            _s.Localization.LanguageChanged += OnLanguageChanged;
            Reload();
        };
        Unloaded += (_, _) => _s.Localization.LanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(object? sender, EventArgs e) => Reload();

    private void Reload()
    {
        // Только работы по тестам, которыми пользователь управляет
        var manageable = _s.Tests.GetAll().Where(_s.Auth.CanManage).Select(t => t.Id).ToHashSet();
        var pending = _s.Results.GetPendingReview().Where(r => manageable.Contains(r.TestId)).ToList();

        PendingGrid.ItemsSource = new ResultRowFactory(_s).Build(pending);
        EmptyText.Visibility = pending.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        ClearDetails();
    }

    private void ClearDetails()
    {
        _current = null;
        _items = new();
        ReviewList.ItemsSource = null;
        HeaderText.Text = "";
        HintText.Visibility = Visibility.Visible;
        SavePanel.Visibility = Visibility.Collapsed;
    }

    private void PendingGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PendingGrid.SelectedItem is not ResultRow row) return;

        var loc = _s.Localization;
        _current = _s.Results.GetById(row.Source.Id);       // свежая копия из хранилища
        if (_current is null) { ClearDetails(); return; }

        HeaderText.Text = $"{row.Student} — {row.Test}";
        HintText.Visibility = Visibility.Collapsed;
        CommentBox.Text = _current.TeacherComment ?? "";

        // Проверяем вручную только те ответы, что не оценены автоматически
        _items = _current.Details.Where(d => !d.IsGraded).Select(d =>
        {
            bool isFile = d.QuestionType == QuestionType.FileUpload;
            return new ReviewItem
            {
                Detail = d,
                QuestionText = d.QuestionText,
                AnswerText = d.GivenTextAnswer,
                FilePath = d.UploadedFilePath ?? "",
                FileLabel = "📎 " + loc["Btn_OpenFile"] + ": " + Path.GetFileName(d.UploadedFilePath ?? ""),
                Reference = string.IsNullOrWhiteSpace(d.Explanation) ? "" : $"{loc["Lbl_Reference"]}: {d.Explanation}",
                MaxText = loc.Format("Lbl_OutOf", d.MaxScore),
                TextVis = isFile ? Visibility.Collapsed : Visibility.Visible,
                FileVis = isFile ? Visibility.Visible : Visibility.Collapsed,
                ReferenceVis = string.IsNullOrWhiteSpace(d.Explanation) ? Visibility.Collapsed : Visibility.Visible,
                ScoreText = "0"
            };
        }).ToList();

        ReviewList.ItemsSource = _items;
        SavePanel.Visibility = Visibility.Visible;
    }

    private void OpenFile_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is not ReviewItem item) return;
        var loc = _s.Localization;

        // Безопасность: открываем только файлы из белого списка расширений (ученик не мог загрузить .exe/.bat)
        if (!File.Exists(item.FilePath) || !AppFiles.IsAllowedUpload(item.FilePath))
        {
            MessageBox.Show(loc["Err_FileOpen"]); return;
        }
        try { Process.Start(new ProcessStartInfo(item.FilePath) { UseShellExecute = true }); }
        catch (Exception) { MessageBox.Show(loc["Err_FileOpen"]); }
    }

    /// <summary>
    /// AI-підказка оцінок для відкритих текстових відповідей. Це лише ПРОПОЗИЦІЯ: бал і коментар підставляються
    /// в поля, викладач редагує їх і сам підтверджує кнопкою «Завершити перевірку». Файли AI не читає.
    /// У запит потрапляють лише текст питання, орієнтир і відповідь — без імені учня.
    /// </summary>
    private async void AiSuggest_Click(object sender, RoutedEventArgs e)
    {
        var loc = _s.Localization;
        var targets = _items.Where(i => i.Detail.QuestionType == QuestionType.TextAnswer).ToList();
        if (targets.Count == 0)
        {
            MessageBox.Show(loc["Ai_NoTextAnswers"]);
            return;
        }

        AiButton.IsEnabled = false;
        AiStatus.Text = loc["Ai_Working"];
        try
        {
            foreach (var item in targets)
            {
                var suggestion = await _s.AiAssistant.SuggestGradeAsync(
                    item.QuestionText, item.Detail.Explanation, item.AnswerText, item.Detail.MaxScore, CancellationToken.None);

                item.ScoreText = suggestion.Score.ToString("0.##", CultureInfo.InvariantCulture);
                item.Note = string.IsNullOrWhiteSpace(suggestion.Comment) ? "" : $"[AI] {suggestion.Comment}";
            }
            AiStatus.Text = loc["Ai_GradesFilled"];
        }
        catch (Exception ex)
        {
            AiStatus.Text = AiErrorFormatter.Describe(loc, ex);
        }
        finally
        {
            AiButton.IsEnabled = true;
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (_current is null) return;
        var loc = _s.Localization;

        // 1. Валидация всех баллов (0..max) до применения
        var parsed = new List<(ReviewItem Item, double Score)>();
        foreach (var item in _items)
        {
            if (!double.TryParse(item.ScoreText.Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double score)
                || score < 0 || score > item.Detail.MaxScore)
            {
                MessageBox.Show(loc.Format("Err_ScoreRange", item.Detail.MaxScore));
                return;
            }
            parsed.Add((item, score));
        }

        // 2. Применяем баллы и завершаем проверку: PendingTeacherReview -> Completed
        foreach (var (item, score) in parsed)
            _s.Grading.ApplyManualScore(item.Detail, score, item.Note);

        _s.Sessions.CompleteReview(_current, _s.Auth.GetCurrentUser()!.Id, CommentBox.Text);

        MessageBox.Show(loc["Msg_ReviewSaved"]);
        Reload();
    }
}
