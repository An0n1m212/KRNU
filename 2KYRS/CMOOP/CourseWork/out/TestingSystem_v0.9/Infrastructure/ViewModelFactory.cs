using Microsoft.Extensions.DependencyInjection;
using TestingSystem.Abstractions;

namespace TestingSystem.Infrastructure;

/// <summary>
/// Фабрика ViewModel: залежності беруться з DI, а специфічні параметри (наприклад, редагована лекція) передаються явно.
/// Єдине місце, де «знають» про контейнер; самі ViewModel залежать лише від абстракцій.
/// </summary>
public sealed class ViewModelFactory : IViewModelFactory
{
    private readonly IServiceProvider _provider;

    public ViewModelFactory(IServiceProvider provider) => _provider = provider;

    public T Create<T>(params object[] args) where T : class
        => ActivatorUtilities.CreateInstance<T>(_provider, args);
}
