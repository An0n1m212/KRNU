using TestingSystem.Abstractions;

namespace TestingSystem.Services;

public sealed class SystemClock : IClock
{
    public DateTime Now => DateTime.Now;
}
