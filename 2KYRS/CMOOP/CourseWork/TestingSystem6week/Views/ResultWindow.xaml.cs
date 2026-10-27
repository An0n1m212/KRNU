using System.Windows;
using TestingSystem.Models;

namespace TestingSystem.Views;

/// <summary>Тиждень 5: підсумок одразу (ТЗ п.3.4.4): складено/не складено або попередній результат.</summary>
public partial class ResultWindow : Window
{
    public ResultWindow(Result result)
    {
        InitializeComponent();
        SummaryText.Text = result.Status == ResultStatus.PendingTeacherReview
            ? $"Попередній результат: {result.TotalScore}/{result.MaxScore} ({result.Percentage}%). Частина відповідей чекає викладача."
            : result.Status == ResultStatus.Expired
                ? $"Час вичерпано: {result.TotalScore}/{result.MaxScore} ({result.Percentage}%)."
                : result.IsPassed
                    ? $"Складено: {result.TotalScore}/{result.MaxScore} ({result.Percentage}%)."
                    : $"Не складено: {result.TotalScore}/{result.MaxScore} ({result.Percentage}%).";
        DetailsText.Text = $"Статус: {result.Status}. Розбір помилок і сертифікат — тиждень 6.";
    }

    private void OnClose(object sender, RoutedEventArgs e) => Close();
}
