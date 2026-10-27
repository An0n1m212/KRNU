using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Services.Ai;

/// <summary>
/// Предметний AI-помічник: формує запит (AiPrompts), викликає IAiClient, розбирає та ВАЛІДУЄ відповідь.
/// Залежить лише від абстракцій; провайдер можна замінити без змін у цьому класі.
/// </summary>
public sealed class AiAssistant : IAiAssistant
{
    private const int MaxSourceChars = 12_000;
    private const int MaxAnswerChars = 4_000;

    private readonly IAiClient _client;
    private readonly IAiSettingsStore _settings;
    private readonly ILocalizationService _loc;
    private readonly IAuthService _auth;

    public AiAssistant(IAiClient client, IAiSettingsStore settings, ILocalizationService loc, IAuthService auth)
    {
        _client = client;
        _settings = settings;
        _loc = loc;
        _auth = auth;
    }

    public string ProviderDescription => _client.Describe();
    public bool IsMock => _settings.Current.Provider == AiProviderKind.Mock;

    /// <summary>Мова відповіді AI = мова інтерфейсу.</summary>
    private string Language => _loc.CurrentLanguage switch
    {
        AppLanguage.UA => "Ukrainian",
        AppLanguage.RU => "Russian",
        _ => "English"
    };

    public async Task<IReadOnlyList<LectureTopicSuggestion>> SuggestTopicsAsync(string subject, int count, CancellationToken ct)
    {
        subject = RequireText(subject, 300);
        count = Math.Clamp(count, 1, 15);

        string raw = await _client.CompleteAsync(AiPrompts.Topics(subject, count, Language), ct);
        var dto = AiJson.Parse<TopicsDto>(raw);

        var topics = (dto.Topics ?? new())
            .Select(t => new LectureTopicSuggestion(AiTestMapper.Clean(t.Title, 200), AiTestMapper.Clean(t.Description, 500)))
            .Where(t => t.Title.Length > 0).Take(count).ToList();

        return topics.Count > 0 ? topics : throw new AiException(AiErrorKind.InvalidResponse, "No topics in the response.");
    }

    public async Task<GeneratedLecture> GenerateLectureAsync(string topic, string? audience, CancellationToken ct)
    {
        topic = RequireText(topic, 300);

        string raw = await _client.CompleteAsync(AiPrompts.Lecture(topic, audience, Language), ct);
        var dto = AiJson.Parse<LectureDto>(raw);

        string content = AiTestMapper.Clean(dto.Content, 60_000);
        if (content.Length == 0) throw new AiException(AiErrorKind.InvalidResponse, "Empty lecture content.");

        return new GeneratedLecture(
            AiTestMapper.Clean(dto.Title, 200) is { Length: > 0 } title ? title : topic,
            AiTestMapper.Clean(dto.Summary, 800),
            content);
    }

    public async Task<GeneratedTestResult> GenerateTestAsync(string topic, string? sourceText, int questionCount,
                                                             IReadOnlyCollection<QuestionType> allowedTypes, CancellationToken ct)
    {
        topic = RequireText(topic, 300);
        questionCount = Math.Clamp(questionCount, 1, 20);
        var allowed = allowedTypes.Where(t => t != QuestionType.FileUpload).ToList();
        if (allowed.Count == 0) throw new ArgumentException("No question types selected.", nameof(allowedTypes));

        string? source = string.IsNullOrWhiteSpace(sourceText) ? null
            : sourceText.Length <= MaxSourceChars ? sourceText : sourceText[..MaxSourceChars];

        string raw = await _client.CompleteAsync(AiPrompts.Test(topic, source, questionCount, allowed, Language), ct);
        var dto = AiJson.Parse<TestDto>(raw);

        // Автор тесту — поточний викладач; нічого не зберігаємо автоматично
        int authorId = _auth.GetCurrentUser()?.Id ?? 0;
        var (test, skipped) = AiTestMapper.ToTest(dto, authorId, topic, allowed);

        if (test.Questions.Count == 0) throw new AiException(AiErrorKind.InvalidResponse, "No valid questions in the response.");
        if (test.Questions.Count > questionCount) test.Questions = test.Questions.Take(questionCount).ToList();

        return new GeneratedTestResult(test, skipped);
    }

    public async Task<AiGradeSuggestion> SuggestGradeAsync(string question, string? reference, string studentAnswer, double maxScore, CancellationToken ct)
    {
        // Приватність: у запит передається лише текст питання та відповіді — без імені студента та інших персональних даних
        string answer = studentAnswer.Length <= MaxAnswerChars ? studentAnswer : studentAnswer[..MaxAnswerChars];
        if (string.IsNullOrWhiteSpace(answer)) return new AiGradeSuggestion(0, string.Empty);

        string raw = await _client.CompleteAsync(AiPrompts.Grade(question, reference, answer, Language), ct);
        var dto = AiJson.Parse<GradeDto>(raw);

        double ratio = double.IsFinite(dto.Ratio) ? Math.Clamp(dto.Ratio, 0, 1) : 0;
        return new AiGradeSuggestion(Math.Round(ratio * maxScore, 2), AiTestMapper.Clean(dto.Comment, 600));
    }

    private static string RequireText(string value, int max)
    {
        string v = (value ?? string.Empty).Trim();
        if (v.Length == 0) throw new ArgumentException("Text is required.", nameof(value));
        return v.Length <= max ? v : v[..max];
    }
}
