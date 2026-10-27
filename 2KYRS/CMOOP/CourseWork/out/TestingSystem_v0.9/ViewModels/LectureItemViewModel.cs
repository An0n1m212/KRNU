using TestingSystem.Infrastructure.Mvvm;
using TestingSystem.Models;

namespace TestingSystem.ViewModels;

/// <summary>Рядок у списку лекцій: заголовок, короткий опис і службова інформація.</summary>
public sealed class LectureItemViewModel : ViewModelBase
{
    public Lecture Source { get; }
    public string Title => Source.Title;
    public string Summary => Source.Summary;
    public string Meta { get; }

    public LectureItemViewModel(Lecture source, string meta)
    {
        Source = source;
        Meta = meta;
    }
}
