using System.Windows;
using TestingSystem.Models;

namespace TestingSystem.Views;

/// <summary>Элемент разбора: ответ ученика, правильный ответ, балл, пояснение.</summary>
public class DetailItem
{
    public string QuestionText { get; init; } = "";
    public string GivenText { get; init; } = "";
    public string CorrectText { get; init; } = "";
    public string ScoreText { get; init; } = "";
    public string Explanation { get; init; } = "";
    public string TeacherNote { get; init; } = "";
    public bool IsCorrect { get; init; }
    public bool IsPending { get; init; }
    public Visibility CorrectVis { get; init; }
    public Visibility ExplanationVis { get; init; }
    public Visibility NoteVis { get; init; }
}

/// <summary>
/// Подробный разбор попытки. Ученик видит правильные ответы и пояснения, только если это разрешил учитель
/// (Test.AllowShowExplanations); учитель и админ видят всё.
/// </summary>
public partial class ResultDetailsWindow : Window
{
    public ResultDetailsWindow(AppServices s, Result result, string testTitle, bool showCorrect)
    {
        InitializeComponent();
        var loc = s.Localization;
        Title = loc.Format("Details_Title", testTitle);

        SummaryText.Text = loc.Format("Details_Summary", result.TotalScore, result.MaxScore, result.Percentage, loc[$"Status_{result.Status}"]);
        CommentText.Text = string.IsNullOrWhiteSpace(result.TeacherComment) ? "" : $"{loc["Lbl_TeacherComment"]}: {result.TeacherComment}";
        HiddenText.Text = showCorrect ? "" : loc["Details_Hidden"];

        string none = loc["Lbl_NoAnswer"];

        DetailsList.ItemsSource = result.Details.Select(d => new DetailItem
        {
            QuestionText = d.QuestionText,
            GivenText = string.IsNullOrWhiteSpace(d.GivenSummary) ? none : d.GivenSummary,
            CorrectText = d.CorrectSummary,
            ScoreText = d.IsGraded
                ? loc.Format("Details_Score", d.ScoreAwarded, d.MaxScore)
                : loc["Details_AwaitingReview"],
            Explanation = d.Explanation,
            TeacherNote = string.IsNullOrWhiteSpace(d.TeacherNote) ? "" : $"{loc["Lbl_TeacherNote"]}: {d.TeacherNote}",
            IsCorrect = d.IsGraded && d.ScoreAwarded >= d.MaxScore - 1e-9,
            IsPending = !d.IsGraded,
            CorrectVis = showCorrect && !string.IsNullOrWhiteSpace(d.CorrectSummary) ? Visibility.Visible : Visibility.Collapsed,
            ExplanationVis = showCorrect && !string.IsNullOrWhiteSpace(d.Explanation) ? Visibility.Visible : Visibility.Collapsed,
            NoteVis = string.IsNullOrWhiteSpace(d.TeacherNote) ? Visibility.Collapsed : Visibility.Visible
        }).ToList();
    }
}
