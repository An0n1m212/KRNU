using TestingSystem.Abstractions;

namespace TestingSystem.Services;

#pragma warning disable CS0067   // подія TimedOut ніколи не піднімається

/// <summary>Заглушка IInactivityMonitor до v0.7: нічого не відстежує (тест-вікно може викликати Suspend()).</summary>
public sealed class NullInactivityMonitor : IInactivityMonitor
{
    public TimeSpan Timeout => TimeSpan.Zero;
    public bool IsRunning => false;
    public event EventHandler? TimedOut;

    public void Start() { }
    public void Stop() { }
    public IDisposable Suspend() => new Scope();

    private sealed class Scope : IDisposable
    {
        public void Dispose() { }
    }
}
