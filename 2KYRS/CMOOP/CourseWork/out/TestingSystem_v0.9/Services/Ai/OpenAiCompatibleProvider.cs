using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TestingSystem.Abstractions;

namespace TestingSystem.Services.Ai;

/// <summary>
/// Будь-який сервіс із протоколом "chat/completions": OpenAI, Azure/OpenRouter-шлюзи, а також локальні
/// моделі (Ollama, LM Studio: BaseUrl на кшталт http://localhost:11434/v1, ключ можна не вказувати).
/// </summary>
public sealed class OpenAiCompatibleProvider : IAiProvider
{
    private readonly HttpClient _http;

    public OpenAiCompatibleProvider(HttpClient http) => _http = http;

    public AiProviderKind Kind => AiProviderKind.OpenAiCompatible;

    public async Task<string> CompleteAsync(AiSettings settings, AiRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(settings.Model)) throw new AiException(AiErrorKind.NotConfigured, "Model is empty.");

        string baseUrl = (string.IsNullOrWhiteSpace(settings.BaseUrl) ? "https://api.openai.com/v1" : settings.BaseUrl).TrimEnd('/');
        var body = new
        {
            model = settings.Model,
            messages = new object[]
            {
                new { role = "system", content = request.SystemPrompt },
                new { role = "user", content = request.UserPrompt }
            }
        };

        using var message = new HttpRequestMessage(HttpMethod.Post, baseUrl + "/chat/completions")
        {
            Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
        };
        if (settings.HasKey) message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);

        string json = await AiHttp.SendAsync(_http, message, ct);

        try
        {
            using var doc = JsonDocument.Parse(json);
            string? text = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
            return string.IsNullOrWhiteSpace(text) ? throw new AiException(AiErrorKind.InvalidResponse, "Empty completion.") : text;
        }
        catch (Exception ex) when (ex is JsonException or KeyNotFoundException or InvalidOperationException or IndexOutOfRangeException)
        {
            throw new AiException(AiErrorKind.InvalidResponse, "Unexpected response format.", ex);
        }
    }
}
