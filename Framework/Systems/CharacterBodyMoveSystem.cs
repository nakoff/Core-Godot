using System.Collections.Generic;
using LooksLike.Ecs;
using Godot;
using LooksLike.Utils;
using LooksLike.Framework.Components;

namespace LooksLike.Framework.Systems;

public class CharacterBodyMoveSystem : SystemBase, IEntitiesPhysicsUpdate
{
    private float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public CharacterBodyMoveSystem() : base(new EcsFilter()
        .With<MoveComponent>()
        .With<CharacterBodyComponent>())
    {
    }

    public void EntitiesPhysicsUpdate(Dictionary<ulong, EcsEntity> entities, float delta)
    {
        foreach (var (_, entity) in entities)
        {
            var characterBodyComponent = entity.GetComponent<CharacterBodyComponent>()!;
            var characterBody = characterBodyComponent.Node!;
            var characterVisual = characterBodyComponent.Visual;
            var move = entity.GetComponent<MoveComponent>()!;
            var velocity = characterBody.Velocity;

            var onFloor = characterBody.IsOnFloor();
            if (!onFloor)
                velocity.Y -= gravity * (float)delta;

            if (move.IsJumping)
                velocity.Y = move.JumpVelocity;

            var inertia = characterBodyComponent.Inertia;
            var direction = -move.Direction;

            move.PrevPosition = characterBody.GlobalPosition;
            var _inertia = onFloor ? inertia : inertia * 0.1f;

            if (direction != Vector3.Zero)
            {
                velocity.X = Mathf.Lerp(characterBody.Velocity.X, direction.X * move.Speed, _inertia);
                velocity.Z = Mathf.Lerp(characterBody.Velocity.Z, direction.Z * move.Speed, _inertia);
            }
            else
            {
                velocity.X = Mathf.MoveToward(characterBody.Velocity.X, 0, inertia * 2f);
                velocity.Z = Mathf.MoveToward(characterBody.Velocity.Z, 0, inertia * 2f);
            }

            characterBody.Velocity = velocity;
            characterBody.MoveAndSlide();

        }
    }
}
