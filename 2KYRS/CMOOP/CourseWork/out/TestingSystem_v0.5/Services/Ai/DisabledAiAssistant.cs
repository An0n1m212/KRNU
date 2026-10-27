using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Services.Ai;

/// <summary>Заглушка IAiAssistant до v0.8: кожен виклик повертає помилку «AI не налаштовано».</summary>
public sealed class DisabledAiAssistant : IAiAssistant
{
    public string ProviderDescription => "—";
    public bool IsMock => false;

    private static Task<T> Fail<T>() => Task.FromException<T>(new AiException(AiErrorKind.NotConfigured));

    public Task<IReadOnlyList<LectureTopicSuggestion>> SuggestTopicsAsync(string subject, int count, CancellationToken ct)
        => Fail<IReadOnlyList<LectureTopicSuggestion>>();

    public Task<GeneratedLecture> GenerateLectureAsync(string topic, string? audience, CancellationToken ct)
        => Fail<GeneratedLecture>();

    public Task<GeneratedTestResult> GenerateTestAsync(string topic, string? sourceText, int questionCount,
                                                       IReadOnlyCollection<QuestionType> allowedTypes, CancellationToken ct)
        => Fail<GeneratedTestResult>();

    public Task<AiGradeSuggestion> SuggestGradeAsync(string question, string? reference, string studentAnswer,
                                                     double maxScore, CancellationToken ct)
        => Fail<AiGradeSuggestion>();
}
