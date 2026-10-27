using System.Windows.Threading;
using TestingSystem.Abstractions;

namespace TestingSystem.Infrastructure;

/// <summary>ITicker на основе DispatcherTimer (тики приходят в UI-потоке).</summary>
public sealed class DispatcherTicker : ITicker
{
    private readonly DispatcherTimer _timer;

    public event EventHandler? Tick;

    public DispatcherTicker(TimeSpan interval)
    {
        _timer = new DispatcherTimer { Interval = interval };
        _timer.Tick += (_, _) => Tick?.Invoke(this, EventArgs.Empty);
    }

    public void Start() => _timer.Start();
    public void Stop() => _timer.Stop();
}
