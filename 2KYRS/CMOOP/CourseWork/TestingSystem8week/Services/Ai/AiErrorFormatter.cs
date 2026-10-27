using TestingSystem.Abstractions;

namespace TestingSystem.Services.Ai;

/// <summary>Перетворює винятки на зрозумілі локалізовані повідомлення для користувача.</summary>
public static class AiErrorFormatter
{
    public static string Describe(ILocalizationService loc, Exception ex) => ex switch
    {
        AiException a when string.IsNullOrWhiteSpace(a.Detail) => loc[$"Ai_Err_{a.Kind}"],
        AiException a => $"{loc[$"Ai_Err_{a.Kind}"]} ({a.Detail})",
        ArgumentException => loc["Ai_Err_Topic"],
        UnauthorizedAccessException => loc["Err_Forbidden"],
        _ => ex.Message
    };
}
