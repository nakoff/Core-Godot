using System.Collections.Generic;
using LooksLike.Ecs;
using Godot;

namespace LooksLike.Framework.Components;

[GlobalClass]
public partial class HitRayDetectComponent : EcsComponent
{
    // [Export] public RayCast3D? Node;
    [Export] public Vector3 TargetPosition;

    public List<ulong> ExcludedEntities { get; set; } = new();

    // public override void _Ready()
    // {
    //     if (Node == null)
    //     {
    //         GD.PrintErr("Hit Detect Node is null");
    //         return;
    //     }
    //
    //     // Will be enabled when the entity is added
    //     Node.Enabled = false;
    // }
}
