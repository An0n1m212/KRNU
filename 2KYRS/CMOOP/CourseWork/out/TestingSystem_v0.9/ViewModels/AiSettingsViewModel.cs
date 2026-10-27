using TestingSystem.Abstractions;
using TestingSystem.Infrastructure.Mvvm;
using TestingSystem.Services.Ai;

namespace TestingSystem.ViewModels;

/// <summary>Налаштування AI-провайдера: тип, адреса, модель, API-ключ, перевірка з'єднання.</summary>
public sealed class AiSettingsViewModel : BusyViewModelBase, IDialogViewModel
{
    private readonly IAiSettingsStore _store;
    private readonly IAiClient _client;

    private Option<AiProviderKind> _selectedProvider;
    private string _baseUrl;
    private string _model;
    private string _apiKey = string.Empty;
    private string _timeoutText;
    private bool _keyRemoved;

    public string Title => Loc["Ai_SettingsTitle"];
    public event EventHandler<bool?>? CloseRequested;

    public IReadOnlyList<Option<AiProviderKind>> Providers { get; }

    public Option<AiProviderKind> SelectedProvider
    {
        get => _selectedProvider;
        set
        {
            if (value is null || value == _selectedProvider) return;
            var old = _selectedProvider.Value;
            if (!SetProperty(ref _selectedProvider, value)) return;

            // Підставляємо типові адресу/модель, якщо поля порожні або містили значення попереднього провайдера
            var oldDefaults = AiProviderDefaults.For(old);
            var newDefaults = AiProviderDefaults.For(value.Value);
            if (string.IsNullOrWhiteSpace(BaseUrl) || BaseUrl == oldDefaults.BaseUrl) BaseUrl = newDefaults.BaseUrl;
            if (string.IsNullOrWhiteSpace(Model) || Model == oldDefaults.Model) Model = newDefaults.Model;
            OnPropertyChanged(nameof(IsExternal));
        }
    }

    /// <summary>false для Mock — поля підключення неактуальні.</summary>
    public bool IsExternal => _selectedProvider.Value != AiProviderKind.Mock;

    public string BaseUrl { get => _baseUrl; set => SetProperty(ref _baseUrl, value ?? string.Empty); }
    public string Model { get => _model; set => SetProperty(ref _model, value ?? string.Empty); }
    public string ApiKey { get => _apiKey; set => SetProperty(ref _apiKey, value ?? string.Empty); }
    public string TimeoutText { get => _timeoutText; set => SetProperty(ref _timeoutText, value ?? string.Empty); }

    public string KeyHint => _store.Current.HasKey && !_keyRemoved ? Loc["Ai_KeyStored"] : Loc["Ai_KeyNotSet"];

    public RelayCommand SaveCommand { get; }
    public RelayCommand CloseCommand { get; }
    public RelayCommand RemoveKeyCommand { get; }
    public AsyncRelayCommand TestCommand { get; }

    public AiSettingsViewModel(IAiSettingsStore store, IAiClient client, ILocalizationService loc) : base(loc)
    {
        _store = store;
        _client = client;

        Providers = new[]
        {
            new Option<AiProviderKind>(AiProviderKind.Mock, loc["Ai_Provider_Mock"]),
            new Option<AiProviderKind>(AiProviderKind.OpenAiCompatible, loc["Ai_Provider_OpenAi"]),
            new Option<AiProviderKind>(AiProviderKind.Anthropic, loc["Ai_Provider_Anthropic"]),
            new Option<AiProviderKind>(AiProviderKind.Gemini, loc["Ai_Provider_Gemini"])
        };

        var s = store.Current;
        _selectedProvider = Providers.First(p => p.Value == s.Provider);
        _baseUrl = s.BaseUrl;
        _model = s.Model;
        _timeoutText = s.TimeoutSeconds.ToString();

        SaveCommand = new RelayCommand(Save, () => !IsBusy);
        CloseCommand = new RelayCommand(() => CloseRequested?.Invoke(this, false));
        RemoveKeyCommand = new RelayCommand(() =>
        {
            _keyRemoved = true;
            ApiKey = string.Empty;
            OnPropertyChanged(nameof(KeyHint));
        });
        TestCommand = new AsyncRelayCommand(TestAsync, () => !IsBusy);
    }

    /// <summary>Перевіряє введені значення; повертає налаштування або ключ локалізованої помилки.</summary>
    private (AiSettings? Settings, string? ErrorKey) Build()
    {
        var kind = _selectedProvider.Value;
        if (!int.TryParse(TimeoutText.Trim(), out int timeout) || timeout < 5 || timeout > 300) return (null, "Err_Ai_Timeout");

        string baseUrl = BaseUrl.Trim();
        string model = Model.Trim();
        string key = _apiKey.Length > 0 ? _apiKey : _keyRemoved ? string.Empty : _store.Current.ApiKey;

        if (kind != AiProviderKind.Mock)
        {
            if (model.Length == 0) return (null, "Err_Ai_Model");

            if (baseUrl.Length > 0)
            {
                if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri) ||
                    (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp)) return (null, "Err_Ai_Url");

                // Ключ та дані не передаємо відкритим HTTP, окрім локальних моделей (localhost)
                if (uri.Scheme == Uri.UriSchemeHttp && !uri.IsLoopback) return (null, "Err_Ai_Https");
            }

            if (kind is AiProviderKind.Anthropic or AiProviderKind.Gemini && key.Length == 0) return (null, "Err_Ai_Key");
        }

        return (new AiSettings(kind, baseUrl, model, key, timeout), null);
    }

    private Task TestAsync() => RunBusyAsync(async ct =>
    {
        var (settings, errorKey) = Build();
        if (settings is null) { ErrorMessage = Loc[errorKey!]; return; }

        StatusMessage = Loc["Ai_Working"];
        string reply = (await _client.CompleteAsync(settings, AiPrompts.Ping(), ct)).Trim();
        StatusMessage = Loc.Format("Ai_ConnectionOk", reply.Length <= 40 ? reply : reply[..40] + "…");
    });

    private void Save()
    {
        ErrorMessage = string.Empty;
        var (settings, errorKey) = Build();
        if (settings is null) { ErrorMessage = Loc[errorKey!]; return; }

        try
        {
            _store.Save(settings);
            CloseRequested?.Invoke(this, true);
        }
        catch (Exception ex)   // немає прав на запис у AppData, DPAPI недоступний тощо
        {
            ErrorMessage = ex.Message;
        }
    }
}

/// <summary>Типові адреса та модель для провайдерів (користувач може змінити).</summary>
public static class AiProviderDefaults
{
    public static (string BaseUrl, string Model) For(AiProviderKind kind) => kind switch
    {
        AiProviderKind.OpenAiCompatible => ("https://api.openai.com/v1", "gpt-4o-mini"),
        AiProviderKind.Anthropic => ("https://api.anthropic.com", "claude-sonnet-5-5"),
        AiProviderKind.Gemini => ("https://generativelanguage.googleapis.com", "gemini-3.8-flash"),
        _ => (string.Empty, string.Empty)
    };
}
