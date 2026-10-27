using System.Collections.ObjectModel;
using TestingSystem.Abstractions;
using TestingSystem.Infrastructure.Mvvm;
using TestingSystem.Models;

namespace TestingSystem.ViewModels;

/// <summary>
/// Вкладка «AI-помічник»: теми лекцій, створення лекції та створення тесту. Усе, що згенерував AI, відкривається
/// в редакторі як чернетка — викладач перевіряє і лише потім зберігає (human-in-the-loop).
/// </summary>
public sealed class AiAssistantViewModel : BusyViewModelBase, IViewLifecycle
{
    private readonly IAiAssistant _ai;
    private readonly ILectureService _lectures;
    private readonly IDialogService _dialogs;
    private readonly IViewModelFactory _factory;
    private readonly ITestEditorLauncher _testEditors;

    private string _providerText = string.Empty;
    private bool _isMock;
    private string _subject = string.Empty;
    private int _topicCount = 5;
    private LectureTopicSuggestion? _selectedSuggestion;
    private string _lectureTopic = string.Empty;
    private string _testTopic = string.Empty;
    private Lecture _selectedSourceLecture = new();
    private int _questionCount = 8;

    public string ProviderText { get => _providerText; private set => SetProperty(ref _providerText, value); }
    public bool IsMockProvider { get => _isMock; private set => SetProperty(ref _isMock, value); }

    // --- теми ---
    public string Subject { get => _subject; set => SetProperty(ref _subject, value ?? string.Empty); }
    public IReadOnlyList<int> TopicCounts { get; } = new[] { 3, 5, 7, 10 };
    public int TopicCount { get => _topicCount; set => SetProperty(ref _topicCount, value); }
    public ObservableCollection<LectureTopicSuggestion> Suggestions { get; } = new();
    public LectureTopicSuggestion? SelectedSuggestion { get => _selectedSuggestion; set => SetProperty(ref _selectedSuggestion, value); }

    // --- лекція ---
    public string LectureTopic { get => _lectureTopic; set => SetProperty(ref _lectureTopic, value ?? string.Empty); }

    // --- тест ---
    public string TestTopic { get => _testTopic; set => SetProperty(ref _testTopic, value ?? string.Empty); }
    public ObservableCollection<Lecture> SourceLectures { get; } = new();
    public Lecture SelectedSourceLecture { get => _selectedSourceLecture; set => SetProperty(ref _selectedSourceLecture, value ?? new Lecture()); }
    public IReadOnlyList<int> QuestionCounts { get; } = new[] { 5, 8, 10, 15, 20 };
    public int QuestionCount { get => _questionCount; set => SetProperty(ref _questionCount, value); }
    public bool IncludeSingle { get; set; } = true;
    public bool IncludeMultiple { get; set; } = true;
    public bool IncludeText { get; set; } = true;
    public bool IncludeMatching { get; set; } = true;
    public bool IncludeSequence { get; set; } = true;

    public RelayCommand OpenSettingsCommand { get; }
    public AsyncRelayCommand SuggestTopicsCommand { get; }
    public AsyncRelayCommand LectureFromSuggestionCommand { get; }
    public AsyncRelayCommand GenerateLectureCommand { get; }
    public AsyncRelayCommand GenerateTestCommand { get; }

    public AiAssistantViewModel(IAiAssistant ai, ILectureService lectures, IDialogService dialogs, IViewModelFactory factory,
                                ITestEditorLauncher testEditors, ILocalizationService loc) : base(loc)
    {
        _ai = ai;
        _lectures = lectures;
        _dialogs = dialogs;
        _factory = factory;
        _testEditors = testEditors;

        OpenSettingsCommand = new RelayCommand(OpenSettings, () => !IsBusy);
        SuggestTopicsCommand = new AsyncRelayCommand(SuggestTopicsAsync, () => !IsBusy);
        LectureFromSuggestionCommand = new AsyncRelayCommand(() => GenerateLectureAsync(SelectedSuggestion?.Title ?? string.Empty),
                                                             () => !IsBusy && SelectedSuggestion != null);
        GenerateLectureCommand = new AsyncRelayCommand(() => GenerateLectureAsync(LectureTopic), () => !IsBusy);
        GenerateTestCommand = new AsyncRelayCommand(GenerateTestAsync, () => !IsBusy);
    }

    public void OnShown()
    {
        Loc.LanguageChanged += OnLanguageChanged;
        RefreshProvider();
        ReloadSourceLectures();
    }

    public void OnHidden() => Loc.LanguageChanged -= OnLanguageChanged;

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        RefreshProvider();
        ReloadSourceLectures();
    }

    private void RefreshProvider()
    {
        ProviderText = Loc.Format("Ai_Provider", _ai.ProviderDescription);
        IsMockProvider = _ai.IsMock;
    }

    private void ReloadSourceLectures()
    {
        int selectedId = SelectedSourceLecture.Id;
        SourceLectures.Clear();
        SourceLectures.Add(new Lecture { Id = 0, Title = Loc["Lect_NoneSelected"] });   // «без джерела»
        foreach (var lecture in _lectures.GetManageable()) SourceLectures.Add(lecture);
        SelectedSourceLecture = SourceLectures.FirstOrDefault(l => l.Id == selectedId) ?? SourceLectures[0];
    }

    private void OpenSettings()
    {
        _dialogs.ShowDialog(_factory.Create<AiSettingsViewModel>());
        RefreshProvider();
    }

    private Task SuggestTopicsAsync() => RunBusyAsync(async ct =>
    {
        if (string.IsNullOrWhiteSpace(Subject)) { ErrorMessage = Loc["Ai_Err_Topic"]; return; }

        StatusMessage = Loc["Ai_Working"];
        var topics = await _ai.SuggestTopicsAsync(Subject, TopicCount, ct);

        Suggestions.Clear();
        foreach (var topic in topics) Suggestions.Add(topic);
        SelectedSuggestion = Suggestions.FirstOrDefault();
        StatusMessage = Loc.Format("Ai_TopicsReady", topics.Count);
    });

    private Task GenerateLectureAsync(string topic) => RunBusyAsync(async ct =>
    {
        if (string.IsNullOrWhiteSpace(topic)) { ErrorMessage = Loc["Ai_Err_Topic"]; return; }

        StatusMessage = Loc["Ai_Working"];
        var generated = await _ai.GenerateLectureAsync(topic.Trim(), null, ct);

        // Не зберігаємо автоматично: відкриваємо редактор із чернеткою (непублікована, без груп)
        var draft = new Lecture
        {
            Title = generated.Title, Summary = generated.Summary, Content = generated.Content,
            IsAiGenerated = true, IsPublished = false
        };
        StatusMessage = string.Empty;

        if (_dialogs.ShowDialog(_factory.Create<LectureEditorViewModel>(draft)) == true)
        {
            StatusMessage = Loc["Ai_LectureSaved"];
            ReloadSourceLectures();
        }
    });

    private Task GenerateTestAsync() => RunBusyAsync(async ct =>
    {
        var types = new List<QuestionType>();
        if (IncludeSingle) types.Add(QuestionType.SingleChoice);
        if (IncludeMultiple) types.Add(QuestionType.MultipleChoice);
        if (IncludeText) types.Add(QuestionType.TextAnswer);
        if (IncludeMatching) types.Add(QuestionType.Matching);
        if (IncludeSequence) types.Add(QuestionType.Sequence);
        if (types.Count == 0) { ErrorMessage = Loc["Ai_NoTypes"]; return; }

        string? source = SelectedSourceLecture.Id > 0 ? SelectedSourceLecture.Content : null;
        string topic = !string.IsNullOrWhiteSpace(TestTopic) ? TestTopic
                     : SelectedSourceLecture.Id > 0 ? SelectedSourceLecture.Title : string.Empty;
        if (string.IsNullOrWhiteSpace(topic)) { ErrorMessage = Loc["Ai_Err_Topic"]; return; }

        StatusMessage = Loc["Ai_Working"];
        var result = await _ai.GenerateTestAsync(topic.Trim(), source, QuestionCount, types, ct);
        StatusMessage = Loc.Format("Ai_TestReady", result.Test.Questions.Count, result.SkippedQuestions);

        // Редактор тестів: викладач перевіряє питання, налаштовує групи/час і зберігає
        _testEditors.ShowTestEditor(result.Test);
    });
}
