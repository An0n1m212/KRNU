using TestingSystem.Models;

namespace TestingSystem.Abstractions;

public enum AiProviderKind { Mock, OpenAiCompatible, Anthropic, Gemini }

public enum AiTask { Ping, LectureTopics, Lecture, Test, GradeAnswer }

public enum AiErrorKind { NotConfigured, Network, Unauthorized, RateLimited, ServerError, InvalidResponse, Timeout }

/// <summary>Помилка роботи з AI. Kind — для локалізованого повідомлення, Detail — технічна підказка (без ключів і секретів).</summary>
public sealed class AiException : Exception
{
    public AiErrorKind Kind { get; }
    public string Detail { get; }

    public AiException(AiErrorKind kind, string detail = "", Exception? inner = null)
        : base($"{kind}: {detail}", inner)
    {
        Kind = kind;
        Detail = detail;
    }
}

/// <summary>
/// Запит до AI. Task і Topic/Language/Count потрібні тестовому провайдеру Mock (він повертає шаблонні відповіді);
/// реальні провайдери використовують лише SystemPrompt та UserPrompt.
/// </summary>
public sealed record AiRequest(
    AiTask Task, string SystemPrompt, string UserPrompt,
    string Topic = "", string Language = "Ukrainian", int Count = 5);

public sealed record AiSettings(AiProviderKind Provider, string BaseUrl, string Model, string ApiKey, int TimeoutSeconds)
{
    public static AiSettings Default => new(AiProviderKind.Mock, "", "", "", 90);
    public bool HasKey => !string.IsNullOrWhiteSpace(ApiKey);
}

/// <summary>Конкретний провайдер (Strategy): Mock, OpenAI-сумісний, Anthropic... Новий провайдер = новий клас.</summary>
public interface IAiProvider
{
    AiProviderKind Kind { get; }
    Task<string> CompleteAsync(AiSettings settings, AiRequest request, CancellationToken ct);
}

/// <summary>Єдина точка виклику AI для застосунку; обирає провайдера за поточними налаштуваннями.</summary>
public interface IAiClient
{
    string Describe();
    Task<string> CompleteAsync(AiRequest request, CancellationToken ct = default);
    /// <summary>Те саме, але з явними налаштуваннями (перевірка з'єднання до збереження).</summary>
    Task<string> CompleteAsync(AiSettings settings, AiRequest request, CancellationToken ct = default);
}

public interface IAiSettingsStore
{
    AiSettings Current { get; }
    void Save(AiSettings settings);
    event EventHandler? Changed;
}

/// <summary>
/// Високорівневий AI-помічник для предметної області. Результати — лише ПРОПОЗИЦІЇ: лекції та тести відкриваються
/// в редакторах для перевірки викладачем, оцінки відкритих відповідей підставляються як підказки.
/// </summary>
public interface IAiAssistant
{
    string ProviderDescription { get; }
    bool IsMock { get; }

    Task<IReadOnlyList<LectureTopicSuggestion>> SuggestTopicsAsync(string subject, int count, CancellationToken ct);
    Task<GeneratedLecture> GenerateLectureAsync(string topic, string? audience, CancellationToken ct);
    Task<GeneratedTestResult> GenerateTestAsync(string topic, string? sourceText, int questionCount,
                                                IReadOnlyCollection<QuestionType> allowedTypes, CancellationToken ct);
    Task<AiGradeSuggestion> SuggestGradeAsync(string question, string? reference, string studentAnswer, double maxScore, CancellationToken ct);
}
