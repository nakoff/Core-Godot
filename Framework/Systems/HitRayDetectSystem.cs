using System.Collections.Generic;
using LooksLike.DependencyInjection;
using LooksLike.Ecs;
using Godot;
using LooksLike.Utils;
using LooksLike.Framework.Components;

namespace LooksLike.Framework.Systems;

public class HitRayDetectSystem : SystemBase, IEntitiesPhysicsUpdate
{
    [Inject] private EcsWorld? _world;

    public HitRayDetectSystem() : base(new EcsFilter()
        .With<Transform3DComponent>()
        .With<HitRayDetectComponent>())
    {
    }

    public void EntitiesPhysicsUpdate(Dictionary<ulong, EcsEntity> entities, float deltaTime)
    {
        foreach (var (_, entity) in entities)
        {
            var transform = entity.GetComponent<Transform3DComponent>()!;
            var ray = entity.GetComponent<HitRayDetectComponent>()!;

            var query = new PhysicsRayQueryParameters3D
            {
                From = transform.Transform3D.Origin,
                To = ray.TargetPosition,
                CollisionMask = uint.MaxValue, // Check all layers
            };

            var spaceState = transform.Node!.GetWorld3D().DirectSpaceState;
            var result = spaceState.IntersectRay(query);
            if (result.Count == 0)
                continue;

            if (result.ContainsKey("collider"))
            {
                var collider = result["collider"].As<CollisionObject3D>();
                var targetEntity = collider.Owner as EcsEntity;
                if (targetEntity != null)
                {
                    GD.Print($"Ray hit {targetEntity.Name}");
                }
            }
        }
    }
}
