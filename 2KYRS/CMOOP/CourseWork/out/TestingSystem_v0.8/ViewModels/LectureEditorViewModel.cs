using System.Collections.ObjectModel;
using TestingSystem.Abstractions;
using TestingSystem.Infrastructure.Mvvm;
using TestingSystem.Models;

namespace TestingSystem.ViewModels;

/// <summary>
/// Діалог створення/редагування лекції (також відкривається з чернеткою від AI). Збереження — лише за кнопкою
/// «Зберегти»; текст від AI не публікується автоматично.
/// </summary>
public sealed class LectureEditorViewModel : BusyViewModelBase, IDialogViewModel
{
    private readonly ILectureService _lectures;
    private readonly ICategoryRepository _categories;
    private readonly IAiAssistant _ai;
    private readonly IDialogService _dialogs;
    private readonly Lecture _lecture;

    private string _lectureTitle;
    private string _summary;
    private string _content;
    private string _categoryText = string.Empty;
    private bool _isPublished;
    private bool _isAiGenerated;
    private Test _selectedRelatedTest;

    public string Title => _lecture.Id == 0 ? Loc["Lect_NewTitle"] : Loc.Format("Lect_EditTitle", _lecture.Title);
    public event EventHandler<bool?>? CloseRequested;

    public IReadOnlyList<Category> Categories { get; }
    public ObservableCollection<SelectableItem<Group>> Groups { get; }
    public IReadOnlyList<Test> RelatedTests { get; }

    public string LectureTitle { get => _lectureTitle; set => SetProperty(ref _lectureTitle, value ?? string.Empty); }
    public string Summary { get => _summary; set => SetProperty(ref _summary, value ?? string.Empty); }
    public string Content { get => _content; set => SetProperty(ref _content, value ?? string.Empty); }
    public string CategoryText { get => _categoryText; set => SetProperty(ref _categoryText, value ?? string.Empty); }
    public bool IsPublished { get => _isPublished; set => SetProperty(ref _isPublished, value); }
    public Test SelectedRelatedTest { get => _selectedRelatedTest; set => SetProperty(ref _selectedRelatedTest, value); }
    public bool IsAiGenerated => _isAiGenerated;

    public RelayCommand SaveCommand { get; }
    public RelayCommand CloseCommand { get; }
    public AsyncRelayCommand GenerateCommand { get; }

    public LectureEditorViewModel(ILectureService lectures, ICategoryRepository categories, IGroupRepository groups,
                                  ITestRepository tests, IAuthService auth, IAiAssistant ai, IDialogService dialogs,
                                  ILocalizationService loc, Lecture lecture) : base(loc)
    {
        _lectures = lectures;
        _categories = categories;
        _ai = ai;
        _dialogs = dialogs;
        _lecture = lecture;

        _lectureTitle = lecture.Title;
        _summary = lecture.Summary;
        _content = lecture.Content;
        _isPublished = lecture.IsPublished;
        _isAiGenerated = lecture.IsAiGenerated;

        Categories = categories.GetAll();
        _categoryText = lecture.CategoryId is int cid ? categories.GetById(cid)?.Name ?? string.Empty : string.Empty;

        Groups = new ObservableCollection<SelectableItem<Group>>(
            groups.GetAll().Select(g => new SelectableItem<Group>(g, g.Name, lecture.AssignedGroupIds.Contains(g.Id))));

        // Тест за темою лекції: перший пункт — «немає»
        var none = new Test { Id = 0, Title = loc["Lect_NoRelatedTest"] };
        RelatedTests = new[] { none }.Concat(tests.GetAll().Where(auth.CanManage)).ToList();
        _selectedRelatedTest = RelatedTests.FirstOrDefault(t => t.Id == lecture.RelatedTestId) ?? none;

        SaveCommand = new RelayCommand(Save, () => !IsBusy);
        CloseCommand = new RelayCommand(() => CloseRequested?.Invoke(this, false));
        GenerateCommand = new AsyncRelayCommand(GenerateAsync, () => !IsBusy);
    }

    private Task GenerateAsync() => RunBusyAsync(async ct =>
    {
        if (string.IsNullOrWhiteSpace(LectureTitle)) { ErrorMessage = Loc["Ai_Err_Topic"]; return; }
        if (!string.IsNullOrWhiteSpace(Content) && !_dialogs.Confirm(Loc["Ai_ConfirmOverwrite"])) return;

        StatusMessage = Loc["Ai_Working"];
        var generated = await _ai.GenerateLectureAsync(LectureTitle.Trim(), null, ct);

        Summary = generated.Summary;
        Content = generated.Content;
        _isAiGenerated = true;
        OnPropertyChanged(nameof(IsAiGenerated));
        StatusMessage = Loc["Ai_LectureReady"];
    });

    private void Save()
    {
        ErrorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(LectureTitle)) { ErrorMessage = Loc["Err_Lect_Title"]; return; }
        if (string.IsNullOrWhiteSpace(Content)) { ErrorMessage = Loc["Err_Lect_Content"]; return; }

        _lecture.Title = LectureTitle.Trim();
        _lecture.Summary = Summary.Trim();
        _lecture.Content = Content;
        _lecture.IsPublished = IsPublished;
        _lecture.IsAiGenerated = _isAiGenerated;
        _lecture.CategoryId = ResolveCategory();
        _lecture.AssignedGroupIds = Groups.Where(g => g.IsSelected).Select(g => g.Item.Id).ToList();
        _lecture.RelatedTestId = SelectedRelatedTest.Id > 0 ? SelectedRelatedTest.Id : null;

        try
        {
            _lectures.Save(_lecture);
            CloseRequested?.Invoke(this, true);
        }
        catch (UnauthorizedAccessException)
        {
            ErrorMessage = Loc["Err_Forbidden"];
        }
    }

    /// <summary>Категорію можна вибрати зі списку або ввести нову — вона буде створена.</summary>
    private int? ResolveCategory()
    {
        string name = CategoryText.Trim();
        if (name.Length == 0) return null;

        var existing = _categories.GetAll().FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
        return (existing ?? _categories.Save(new Category { Name = name })).Id;
    }
}
