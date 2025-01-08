using System.Collections.Generic;
using LooksLike.DependencyInjection;
using LooksLike.Ecs;
using LooksLike.Utils;
using LooksLike.Framework.Components;

namespace LooksLike.Framework.Systems;

public class TriggerDetectSystem : SystemBase, IEntitiesPhysicsUpdate
{
    [Inject] private EcsWorld? _world;

    public TriggerDetectSystem() : base(new EcsFilter()
        .With<TriggerDetectComponent>())
    {
    }

    public void EntitiesPhysicsUpdate(Dictionary<ulong, EcsEntity> entities, float deltaTime)
    {
        foreach (var (_, entity) in entities)
        {
            var collisionDetect = entity.GetComponent<TriggerDetectComponent>()!;
            var otherBody = collisionDetect.DetectedBody;

            if (collisionDetect.Timeout > 0)
            {
                collisionDetect.Timeout -= deltaTime;
                continue;
            }

            if (collisionDetect.DetectedBody == null)
                continue;

            collisionDetect.HitsToRemove--;
            collisionDetect.DetectedBody = null;
            if (otherBody?.Owner is EcsEntity otherEntity)
            {
                var damage = entity.GetComponent<DamageComponent>();
                if (damage != null)
                    otherEntity.AddComponent(new DamageReceiveComponent(damage.Value));
            }

            if (collisionDetect.HitsToRemove <= 0)
                _world!.RemoveEntity(entity);
        }
    }
}
