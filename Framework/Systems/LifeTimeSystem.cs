using System.Collections.Generic;
using LooksLike.DependencyInjection;
using LooksLike.Ecs;
using LooksLike.Framework.Components;
using LooksLike.Utils;

namespace LooksLike.Framework.Systems;

public class LifeTimeSystem : SystemBase, IEntitiesUpdate
{
    [Inject] private EcsWorld _world = null!;

    public LifeTimeSystem() : base(new EcsFilter()
        .With<LifeTimeComponent>())
    {
    }

    public void EntitiesUpdate(Dictionary<ulong, EcsEntity> entities, float delta)
    {
        foreach (var (_, entity) in entities)
        {
            var lifeTime = entity.GetComponent<LifeTimeComponent>()!;

            lifeTime.ValueSeconds -= delta;
            if (lifeTime.ValueSeconds <= 0)
                _world.RemoveEntity(entity);
        }
    }
}
