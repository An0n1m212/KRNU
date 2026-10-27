using TestingSystem.Abstractions;

namespace TestingSystem.Services.Ai;

/// <summary>
/// IAiClient, що делегує запит провайдеру, обраному в налаштуваннях (Strategy).
/// Додати нового провайдера = зареєструвати ще одну реалізацію IAiProvider; цей клас не змінюється (OCP).
/// </summary>
public sealed class AiClientRouter : IAiClient
{
    private readonly IReadOnlyList<IAiProvider> _providers;
    private readonly IAiSettingsStore _store;

    public AiClientRouter(IEnumerable<IAiProvider> providers, IAiSettingsStore store)
    {
        _providers = providers.ToList();
        _store = store;
    }

    public string Describe()
    {
        var s = _store.Current;
        return s.Provider == AiProviderKind.Mock ? "Mock (demo)" : $"{s.Provider} · {s.Model}";
    }

    public Task<string> CompleteAsync(AiRequest request, CancellationToken ct = default)
        => CompleteAsync(_store.Current, request, ct);

    public async Task<string> CompleteAsync(AiSettings settings, AiRequest request, CancellationToken ct = default)
    {
        var provider = _providers.FirstOrDefault(p => p.Kind == settings.Provider)
            ?? throw new AiException(AiErrorKind.NotConfigured, $"No provider registered for {settings.Provider}.");

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(Math.Clamp(settings.TimeoutSeconds, 5, 600)));

        try
        {
            return await provider.CompleteAsync(settings, request, cts.Token);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            throw new AiException(AiErrorKind.Timeout, $"{settings.TimeoutSeconds} s");   // це таймаут, а не скасування користувачем
        }
    }
}
