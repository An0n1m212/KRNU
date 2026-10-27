using System.Collections.ObjectModel;
using TestingSystem.Abstractions;
using TestingSystem.Infrastructure.Mvvm;
using TestingSystem.Models;

namespace TestingSystem.ViewModels;

/// <summary>Вкладка викладача «Лекції»: список власних лекцій, створення, редагування, видалення.</summary>
public sealed class LecturesViewModel : ViewModelBase, IViewLifecycle
{
    private readonly ILectureService _lectures;
    private readonly ICategoryRepository _categories;
    private readonly IGroupRepository _groups;
    private readonly ILocalizationService _loc;
    private readonly IDialogService _dialogs;
    private readonly IViewModelFactory _factory;
    private LectureItemViewModel? _selectedItem;

    public ObservableCollection<LectureItemViewModel> Items { get; } = new();

    public LectureItemViewModel? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    public RelayCommand NewCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand DeleteCommand { get; }

    public LecturesViewModel(ILectureService lectures, ICategoryRepository categories, IGroupRepository groups,
                             ILocalizationService loc, IDialogService dialogs, IViewModelFactory factory)
    {
        _lectures = lectures;
        _categories = categories;
        _groups = groups;
        _loc = loc;
        _dialogs = dialogs;
        _factory = factory;

        NewCommand = new RelayCommand(() => OpenEditor(new Lecture()));
        EditCommand = new RelayCommand(Edit, () => SelectedItem != null);
        DeleteCommand = new RelayCommand(Delete, () => SelectedItem != null);
    }

    public void OnShown()
    {
        _loc.LanguageChanged += OnLanguageChanged;
        Reload();
    }

    public void OnHidden() => _loc.LanguageChanged -= OnLanguageChanged;

    private void OnLanguageChanged(object? sender, EventArgs e) => Reload();

    public void Reload()
    {
        int? selectedId = SelectedItem?.Source.Id;
        var categories = _categories.GetAll().ToDictionary(c => c.Id, c => c.Name);
        var groups = _groups.GetAll().ToDictionary(g => g.Id, g => g.Name);

        Items.Clear();
        foreach (var lecture in _lectures.GetManageable())
            Items.Add(new LectureItemViewModel(lecture, BuildMeta(lecture, categories, groups)));

        SelectedItem = Items.FirstOrDefault(i => i.Source.Id == selectedId);
    }

    private string BuildMeta(Lecture l, Dictionary<int, string> categories, Dictionary<int, string> groups)
    {
        var parts = new List<string> { _loc[l.IsPublished ? "Lect_Published" : "Lect_Draft"] };
        if (l.CategoryId is int c && categories.TryGetValue(c, out var categoryName)) parts.Add(categoryName);
        parts.Add(l.AssignedGroupIds.Count == 0
            ? _loc["Meta_AllGroups"]
            : string.Join(", ", l.AssignedGroupIds.Select(id => groups.TryGetValue(id, out var gn) ? gn : "?")));
        parts.Add(l.UpdatedAt.ToString("dd.MM.yyyy HH:mm"));
        if (l.IsAiGenerated) parts.Add("✨ AI");
        return string.Join("  •  ", parts);
    }

    private void Edit()
    {
        if (SelectedItem is null) return;
        var lecture = _lectures.GetById(SelectedItem.Source.Id);
        if (lecture is null) { _dialogs.ShowMessage(_loc["Err_Forbidden"]); return; }
        OpenEditor(lecture);
    }

    private void OpenEditor(Lecture lecture)
    {
        var editor = _factory.Create<LectureEditorViewModel>(lecture);
        if (_dialogs.ShowDialog(editor) == true) Reload();
    }

    private void Delete()
    {
        if (SelectedItem is null) return;
        if (!_dialogs.Confirm(_loc.Format("Lect_ConfirmDelete", SelectedItem.Title))) return;

        try { _lectures.Delete(SelectedItem.Source.Id); }
        catch (UnauthorizedAccessException) { _dialogs.ShowMessage(_loc["Err_Forbidden"]); return; }
        Reload();
    }
}
