using System.Collections.Generic;

namespace LooksLike.Ecs;

public interface IEcsSystem
{
    public EcsFilter Filter { get; }
}

public interface IEntityAdded : IEcsSystem
{
    void EntitiesAdded(List<EcsEntity> entities);
}

public interface IEntitiesUpdate : IEcsSystem
{
    void EntitiesUpdate(Dictionary<ulong, EcsEntity> entities, float deltaTime);
}

public interface IEntitiesPhysicsUpdate : IEcsSystem
{
    void EntitiesPhysicsUpdate(Dictionary<ulong, EcsEntity> entities, float deltaTime);
}

public interface IEntityRemoved : IEcsSystem
{
    void EntitiesRemoved(List<int> ids);
}

public abstract class EcsSystem
{
    public int Id { get; }
    public EcsFilter Filter { get; }

    public EcsSystem(EcsFilter filter)
    {
        Filter = filter;
        Id = GetHashCode();
    }
}
