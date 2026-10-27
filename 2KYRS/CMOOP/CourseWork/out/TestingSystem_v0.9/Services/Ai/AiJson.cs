using System.Text.Json;
using System.Text.Json.Serialization;
using TestingSystem.Abstractions;

namespace TestingSystem.Services.Ai;

/// <summary>
/// Відповідь AI — НЕНАДІЙНІ дані: може містити ```-огорожі, пояснення навколо JSON, зайві коми.
/// Тут витягуємо JSON-об'єкт і десеріалізуємо його; будь-яка проблема стає AiException(InvalidResponse).
/// </summary>
internal static class AiJson
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    public static T Parse<T>(string raw) where T : class
    {
        string json = ExtractObject(raw);
        try
        {
            return JsonSerializer.Deserialize<T>(json, Options)
                   ?? throw new AiException(AiErrorKind.InvalidResponse, "Empty JSON.");
        }
        catch (JsonException ex)
        {
            throw new AiException(AiErrorKind.InvalidResponse, "JSON: " + ex.Message, ex);
        }
    }

    private static string ExtractObject(string raw)
    {
        int start = raw.IndexOf('{');
        int end = raw.LastIndexOf('}');
        if (start < 0 || end <= start) throw new AiException(AiErrorKind.InvalidResponse, "No JSON object in the response.");
        return raw[start..(end + 1)];
    }
}
