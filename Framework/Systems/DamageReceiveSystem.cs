using System.Collections.Generic;
using LooksLike.DependencyInjection;
using LooksLike.Ecs;
using LooksLike.Utils;
using LooksLike.Framework.Components;

namespace Game.Systems;

public class DamageReceiveSystem : SystemBase, IEntitiesUpdate
{
    [Inject] private EcsWorld _world = null!;

    public DamageReceiveSystem() : base(new EcsFilter()
        .With<DamageReceiveComponent>()
        .Without<DeathMarkerComponent>())
    {
    }

    public void EntitiesUpdate(Dictionary<ulong, EcsEntity> entities, float deltaTime)
    {
        foreach (var (_, entity) in entities)
        {
            var damage = entity.GetComponent<DamageReceiveComponent>()!;
            entity.RemoveComponent<DamageReceiveComponent>();

            var health = entity.GetComponent<HealthComponent>();
            if (health != null)
            {
                health.Value -= damage.Value;
                if (health.Value <= 0)
                    entity.AddComponent(new DeathMarkerComponent());
            }
        }
    }
}
