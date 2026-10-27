using System.Collections.ObjectModel;
using TestingSystem.Abstractions;
using TestingSystem.Infrastructure.Mvvm;
using TestingSystem.Models;

namespace TestingSystem.ViewModels;

/// <summary>Вкладка студента «Лекції»: опубліковані лекції його групи та читання вибраної.</summary>
public sealed class StudentLecturesViewModel : ViewModelBase, IViewLifecycle
{
    private readonly ILectureService _lectures;
    private readonly IUserRepository _users;
    private readonly ICategoryRepository _categories;
    private readonly ITestRepository _tests;
    private readonly ILocalizationService _loc;
    private LectureItemViewModel? _selectedItem;

    public ObservableCollection<LectureItemViewModel> Items { get; } = new();

    public LectureItemViewModel? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (!SetProperty(ref _selectedItem, value)) return;
            OnPropertyChanged(nameof(HasSelection));
            OnPropertyChanged(nameof(SelectedTitle));
            OnPropertyChanged(nameof(SelectedInfo));
            OnPropertyChanged(nameof(SelectedContent));
            OnPropertyChanged(nameof(RelatedTestText));
        }
    }

    public bool IsEmpty => Items.Count == 0;
    public bool HasSelection => _selectedItem != null;
    public string SelectedTitle => _selectedItem?.Title ?? string.Empty;
    public string SelectedContent => _selectedItem?.Source.Content ?? string.Empty;

    public string SelectedInfo
    {
        get
        {
            if (_selectedItem is null) return string.Empty;
            var lecture = _selectedItem.Source;
            var parts = new List<string>();

            string? author = _users.GetById(lecture.AuthorTeacherId)?.FullName;
            if (!string.IsNullOrEmpty(author)) parts.Add(_loc.Format("Lect_By", author));
            if (lecture.CategoryId is int c && _categories.GetById(c)?.Name is { } category) parts.Add(category);
            parts.Add(lecture.UpdatedAt.ToString("dd.MM.yyyy"));
            return string.Join("  •  ", parts);
        }
    }

    public string RelatedTestText
        => _selectedItem?.Source.RelatedTestId is int id && _tests.GetById(id) is { } test
            ? _loc.Format("Lect_RelatedTest", test.Title)
            : string.Empty;

    public StudentLecturesViewModel(ILectureService lectures, IUserRepository users, ICategoryRepository categories,
                                    ITestRepository tests, ILocalizationService loc)
    {
        _lectures = lectures;
        _users = users;
        _categories = categories;
        _tests = tests;
        _loc = loc;
    }

    public void OnShown()
    {
        _loc.LanguageChanged += OnLanguageChanged;
        Reload();
    }

    public void OnHidden() => _loc.LanguageChanged -= OnLanguageChanged;

    private void OnLanguageChanged(object? sender, EventArgs e) => Reload();

    private void Reload()
    {
        int? selectedId = SelectedItem?.Source.Id;

        Items.Clear();
        foreach (var lecture in _lectures.GetForCurrentStudent())
            Items.Add(new LectureItemViewModel(lecture, lecture.UpdatedAt.ToString("dd.MM.yyyy")));

        SelectedItem = Items.FirstOrDefault(i => i.Source.Id == selectedId) ?? Items.FirstOrDefault();
        OnPropertyChanged(nameof(IsEmpty));
    }
}
