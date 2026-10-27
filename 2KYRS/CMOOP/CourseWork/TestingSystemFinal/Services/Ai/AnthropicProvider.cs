using System.Net.Http;
using System.Text;
using System.Text.Json;
using TestingSystem.Abstractions;

namespace TestingSystem.Services.Ai;

/// <summary>Anthropic Messages API (POST {BaseUrl}/v1/messages, заголовки x-api-key та anthropic-version).</summary>
public sealed class AnthropicProvider : IAiProvider
{
    private const int MaxTokens = 8192;
    private readonly HttpClient _http;

    public AnthropicProvider(HttpClient http) => _http = http;

    public AiProviderKind Kind => AiProviderKind.Anthropic;

    public async Task<string> CompleteAsync(AiSettings settings, AiRequest request, CancellationToken ct)
    {
        if (!settings.HasKey) throw new AiException(AiErrorKind.NotConfigured, "API key is empty.");
        if (string.IsNullOrWhiteSpace(settings.Model)) throw new AiException(AiErrorKind.NotConfigured, "Model is empty.");

        string baseUrl = (string.IsNullOrWhiteSpace(settings.BaseUrl) ? "https://api.anthropic.com" : settings.BaseUrl).TrimEnd('/');
        var body = new
        {
            model = settings.Model,
            max_tokens = MaxTokens,
            system = request.SystemPrompt,
            messages = new object[] { new { role = "user", content = request.UserPrompt } }
        };

        using var message = new HttpRequestMessage(HttpMethod.Post, baseUrl + "/v1/messages")
        {
            Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
        };
        message.Headers.Add("x-api-key", settings.ApiKey);
        message.Headers.Add("anthropic-version", "2023-06-01");

        string json = await AiHttp.SendAsync(_http, message, ct);

        try
        {
            using var doc = JsonDocument.Parse(json);
            var text = new StringBuilder();
            foreach (var block in doc.RootElement.GetProperty("content").EnumerateArray())
                if (block.TryGetProperty("type", out var type) && type.GetString() == "text")
                    text.Append(block.GetProperty("text").GetString());

            return text.Length == 0 ? throw new AiException(AiErrorKind.InvalidResponse, "Empty completion.") : text.ToString();
        }
        catch (Exception ex) when (ex is JsonException or KeyNotFoundException or InvalidOperationException)
        {
            throw new AiException(AiErrorKind.InvalidResponse, "Unexpected response format.", ex);
        }
    }
}
