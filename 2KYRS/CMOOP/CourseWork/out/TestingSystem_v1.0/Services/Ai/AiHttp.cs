using System.Net.Http;
using System.Text.Json;
using TestingSystem.Abstractions;

namespace TestingSystem.Services.Ai;

/// <summary>Спільна відправка HTTP-запитів до AI та перетворення помилок (без витоку ключів у повідомленнях).</summary>
internal static class AiHttp
{
    public static async Task<string> SendAsync(HttpClient http, HttpRequestMessage request, CancellationToken ct)
    {
        HttpResponseMessage response;
        try
        {
            response = await http.SendAsync(request, ct);
        }
        catch (HttpRequestException ex)
        {
            throw new AiException(AiErrorKind.Network, ex.Message, ex);
        }

        using (response)
        {
            string body = await response.Content.ReadAsStringAsync(ct);
            if (response.IsSuccessStatusCode) return body;

            int code = (int)response.StatusCode;
            var kind = code is 401 or 403 ? AiErrorKind.Unauthorized
                     : code == 429 ? AiErrorKind.RateLimited
                     : code >= 500 ? AiErrorKind.ServerError
                     : AiErrorKind.InvalidResponse;

            throw new AiException(kind, $"HTTP {code}: {Truncate(ExtractError(body), 300)}");
        }
    }

    /// <summary>Дістає текст помилки зі стандартного формату {"error":{"message":"..."}} або повертає початок тіла.</summary>
    private static string ExtractError(string body)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("error", out var error))
            {
                if (error.ValueKind == JsonValueKind.String) return error.GetString() ?? body;
                if (error.TryGetProperty("message", out var message)) return message.GetString() ?? body;
            }
        }
        catch (JsonException) { }
        return body;
    }

    private static string Truncate(string s, int max) => s.Length <= max ? s : s[..max] + "…";
}
