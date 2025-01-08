using System.Collections.Generic;
using LooksLike.DependencyInjection;
using LooksLike.Ecs;
using Godot;
using LooksLike.Utils;
using LooksLike.Framework.Components;

namespace LooksLike.Framework.Systems;

public class HitDetectSystem : SystemBase, IEntitiesAdded, IEntitiesPhysicsUpdate
{
    [Inject] private EcsWorld? _world;

    public HitDetectSystem() : base(new EcsFilter()
        .With<MoveComponent>()
        .With<Transform3DComponent>()
        .With<HitDetectComponent>())
    {
    }

    public void EntitiesAdded(Dictionary<ulong, EcsEntity> entities)
    {
        foreach (var (_, entity) in entities)
        {
            var collisionDetect = entity.GetComponent<HitDetectComponent>()!;
            collisionDetect.Node!.Enabled = true;
        }
    }

    public void EntitiesPhysicsUpdate(Dictionary<ulong, EcsEntity> entities, float deltaTime)
    {
        foreach (var (_, entity) in entities)
        {
            var move = entity.GetComponent<MoveComponent>()!;
            var currentPosition = entity.GetComponent<Transform3DComponent>()!.Transform3D.Origin;
            var collisionDetect = entity.GetComponent<HitDetectComponent>()!;

            var prevPosition = move.PrevPosition;
            var node = collisionDetect.Node!;
            var distance = prevPosition.DistanceTo(currentPosition);

            if (collisionDetect.Timeout > 0)
            {
                collisionDetect.Timeout -= deltaTime;
                continue;
            }

            node.TargetPosition = new Vector3(0, 0, -distance * 20);

            if (node.IsColliding())
            {
                collisionDetect.HitsToRemove--;
                var colliderNode = node.GetCollider(0) as Node;

                if (colliderNode?.Owner is EcsEntity otherEntity)
                {
                    if (otherEntity.Id == entity.Id)
                        continue;

                    if (collisionDetect.ExcludedEntities.Contains(otherEntity.Id))
                        continue;

                    var damage = entity.GetComponent<DamageComponent>();
                    if (damage != null)
                    {
                        otherEntity.AddComponent(new DamageReceiveComponent(damage.Value));
                    }

                }

                if (collisionDetect.HitsToRemove <= 0)
                {
                    _world!.RemoveEntity(entity);
                }
            }
        }
    }
}
