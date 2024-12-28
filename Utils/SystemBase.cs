using System.Collections.Generic;
using Core.DependencyInjection;

namespace Core.LLEcs;

public class SystemBase : EcsSystem
{
    protected static DIContainer? Container { get; private set; }
    private static List<EcsSystem> _systems = new();

    public static void Initialize(DIContainer container)
    {
        Container = container;

        foreach (var system in _systems)
            container.InjectDependencies(system);
    }

    public SystemBase(EcsFilter filter) : base(filter)
    {
        _systems.Add(this);
    }
}
