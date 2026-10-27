using System.Net.Http;
using System.Text;
using System.Text.Json;
using TestingSystem.Abstractions;

namespace TestingSystem.Services.Ai;

/// <summary>
/// Google Gemini API (POST {BaseUrl}/v1beta/models/{model}:generateContent?key=API_KEY).
/// Ключ: https://aistudio.google.com/apikey. Безкоштовний ліміт достатній для генерації лекцій/тестів.
/// Тіло/відповідь — формат generateContent: system_instruction + contents, текст дістаємо з candidates[0].content.parts.
/// </summary>
public sealed class GeminiProvider : IAiProvider
{
    private const string DefaultBaseUrl = "https://generativelanguage.googleapis.com";
    private const int MaxOutputTokens = 8192;
    private readonly HttpClient _http;

    public GeminiProvider(HttpClient http) => _http = http;

    public AiProviderKind Kind => AiProviderKind.Gemini;

    public async Task<string> CompleteAsync(AiSettings settings, AiRequest request, CancellationToken ct)
    {
        if (!settings.HasKey) throw new AiException(AiErrorKind.NotConfigured, "API key is empty.");
        if (string.IsNullOrWhiteSpace(settings.Model)) throw new AiException(AiErrorKind.NotConfigured, "Model is empty.");

        string baseUrl = NormalizeBase(settings.BaseUrl);
        string url = $"{baseUrl}/v1beta/models/{Uri.EscapeDataString(settings.Model.Trim())}:generateContent?key={Uri.EscapeDataString(settings.ApiKey)}";

        // Для Ping потрібна звичайна текстова відповідь "OK", тому JSON-режим вмикаємо лише для контентних задач.
        // Ліміт Ping навмисно великий (1024, а не 32): reasoning-моделі (Gemini 3) витрачають бюджет
        // на thought-токени, інакше повертають порожній content із finishReason=MAX_TOKENS.
        object generationConfig = request.Task == AiTask.Ping
            ? new { temperature = 0.0, maxOutputTokens = 1024 }
            : new { temperature = 0.7, maxOutputTokens = MaxOutputTokens, responseMimeType = "application/json" };

        var body = new
        {
            system_instruction = new { parts = new object[] { new { text = request.SystemPrompt } } },
            contents = new object[] { new { role = "user", parts = new object[] { new { text = request.UserPrompt } } } },
            generationConfig
        };

        using var message = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
        };

        string json;
        try
        {
            json = await AiHttp.SendAsync(_http, message, ct);
        }
        catch (AiException ex) when (ex.Kind == AiErrorKind.InvalidResponse && ex.Detail.StartsWith("HTTP 404", StringComparison.Ordinal))
        {
            // 404 у Gemini майже завжди означає невірну адресу або назву моделі — підказуємо це прямо в деталі.
            throw new AiException(ex.Kind, ex.Detail + " — check BaseUrl (host only, no /v1beta/models/...) and model name.", ex);
        }

        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Блокування фільтром безпеки (promptFeedback.blockReason) або порожні candidates — це InvalidResponse, а не успіх.
            if (!root.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
            {
                string reason = root.TryGetProperty("promptFeedback", out var feedback) &&
                                feedback.TryGetProperty("blockReason", out var block)
                    ? $"Blocked: {block.GetString()}"
                    : "No candidates in the response.";
                throw new AiException(AiErrorKind.InvalidResponse, reason);
            }

            var text = new StringBuilder();
            string? finishReason = candidates[0].TryGetProperty("finishReason", out var fr) ? fr.GetString() : null;
            var content = candidates[0].GetProperty("content");
            if (content.TryGetProperty("parts", out var parts))
                foreach (var part in parts.EnumerateArray())
                {
                    if (part.ValueKind != JsonValueKind.Object) continue;
                    if (part.TryGetProperty("text", out var t) && t.ValueKind == JsonValueKind.String)
                        text.Append(t.GetString());
                }

            if (text.Length == 0)
            {
                // Тексту немає (порожні parts, safety-блок без blockReason, новий формат полів):
                // кладемо в деталь причину зупинки та початок сирої відповіді, щоб було видно реальну форму.
                string? block = root.TryGetProperty("promptFeedback", out var feedback) &&
                                feedback.TryGetProperty("blockReason", out var blocked)
                    ? blocked.GetString() : null;
                string snippet = json.Length <= 300 ? json : json[..300] + "…";
                int partsCount = -1;
                if (content.TryGetProperty("parts", out var partsArr) && partsArr.ValueKind == JsonValueKind.Array)
                    partsCount = partsArr.GetArrayLength();
                throw new AiException(AiErrorKind.InvalidResponse,
                    "Empty completion (finishReason=" + (finishReason ?? "?") +
                    ", blockReason=" + (block ?? "?") +
                    ", parts=" + partsCount + "). Response: " + snippet);
            }
            return text.ToString();
        }
        catch (Exception ex) when (ex is JsonException or KeyNotFoundException or InvalidOperationException)
        {
            throw new AiException(AiErrorKind.InvalidResponse, "Unexpected response format.", ex);
        }
    }

    /// <summary>
    /// У поле адреси потрібен лише базовий хост, але користувачі часто вставляють повний URL ендпоінта
    /// (.../v1beta/models/xxx:generateContent). Відрізаємо все з /v1beta — модель завжди беремо з поля Model.
    /// </summary>
    private static string NormalizeBase(string baseUrl)
    {
        string raw = (string.IsNullOrWhiteSpace(baseUrl) ? DefaultBaseUrl : baseUrl).Trim().TrimEnd('/');
        int v1 = raw.IndexOf("/v1beta", StringComparison.OrdinalIgnoreCase);
        string root = (v1 >= 0 ? raw[..v1] : raw).TrimEnd('/');
        return root.Length == 0 ? DefaultBaseUrl : root;
    }
}
