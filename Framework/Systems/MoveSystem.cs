using System.Collections.Generic;
using LooksLike.Ecs;
using Godot;
using LooksLike.Utils;
using LooksLike.Framework.Components;

namespace Game.Systems;

public class MoveSystem : SystemBase, IEntitiesUpdate
{
    private float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public MoveSystem() : base(new EcsFilter()
        .With<MoveComponent>()
        .With<Transform3DComponent>()
        .Without<CharacterBodyComponent>())
    {
    }

    public void EntitiesUpdate(Dictionary<ulong, EcsEntity> entities, float delta)
    {
        foreach (var (_, entity) in entities)
        {
            var move = entity.GetComponent<MoveComponent>()!;
            var tr = entity.GetComponent<Transform3DComponent>()!;

            move.PrevPosition = tr.Transform3D.Origin;
            var forward = tr.Transform3D.Basis.Z.Normalized();
            move.Direction = forward;

            tr.Transform3D = new Transform3D(
                tr.Transform3D.Basis,
                tr.Transform3D.Origin + forward * move.Speed * delta
            );
        }
    }
}
