using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TestingSystem.Abstractions;

namespace TestingSystem.Services.Ai;

/// <summary>
/// Налаштування AI у %AppData%\TestingSystem\ai-settings.json. API-ключ шифрується DPAPI
/// (розшифрувати може лише той самий користувач Windows на цьому ПК) і ніколи не пишеться відкритим текстом.
/// Запасний варіант: змінна середовища TESTING_SYSTEM_AI_KEY. У продакшені замість файлу — Key Vault/секрети сервера.
/// </summary>
public sealed class JsonAiSettingsStore : IAiSettingsStore
{
    private sealed class FileModel
    {
        public string Provider { get; set; } = nameof(AiProviderKind.Mock);
        public string BaseUrl { get; set; } = "";
        public string Model { get; set; } = "";
        public string? ProtectedApiKey { get; set; }
        public int TimeoutSeconds { get; set; } = 90;
    }

    private static readonly string FilePath = Path.Combine(AppFiles.Root, "ai-settings.json");
    private AiSettings _current;

    public event EventHandler? Changed;
    public AiSettings Current => _current;

    public JsonAiSettingsStore() => _current = Load();

    public void Save(AiSettings settings)
    {
        var model = new FileModel
        {
            Provider = settings.Provider.ToString(),
            BaseUrl = settings.BaseUrl,
            Model = settings.Model,
            TimeoutSeconds = settings.TimeoutSeconds,
            ProtectedApiKey = settings.HasKey
                ? Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes(settings.ApiKey), null, DataProtectionScope.CurrentUser))
                : null
        };

        Directory.CreateDirectory(AppFiles.Root);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true }));

        _current = settings;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private static AiSettings Load()
    {
        string envKey = Environment.GetEnvironmentVariable("TESTING_SYSTEM_AI_KEY") ?? "";
        try
        {
            if (!File.Exists(FilePath)) return AiSettings.Default with { ApiKey = envKey };

            var model = JsonSerializer.Deserialize<FileModel>(File.ReadAllText(FilePath)) ?? new FileModel();
            string key = envKey;
            if (!string.IsNullOrEmpty(model.ProtectedApiKey))
                key = Encoding.UTF8.GetString(ProtectedData.Unprotect(Convert.FromBase64String(model.ProtectedApiKey), null, DataProtectionScope.CurrentUser));

            return new AiSettings(
                Enum.TryParse<AiProviderKind>(model.Provider, out var kind) ? kind : AiProviderKind.Mock,
                model.BaseUrl, model.Model, key, Math.Clamp(model.TimeoutSeconds, 5, 600));
        }
        catch (Exception ex) when (ex is IOException or JsonException or CryptographicException or FormatException or UnauthorizedAccessException)
        {
            return AiSettings.Default;   // пошкоджений файл або ключ від іншого користувача — безпечний режим Mock
        }
    }
}
