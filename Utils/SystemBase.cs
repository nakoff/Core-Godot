using Core.DependencyInjection;

namespace Core.LLEcs;

public class SystemBase : EcsSystem
{
    public SystemBase(DIContainer container, EcsFilter filter) : base(filter)
    {
        container.InjectDependencies(this);
    }
}
