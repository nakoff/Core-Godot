using System.Collections.Generic;
using LooksLike.Ecs;
using Godot;

namespace LooksLike.Framework.Components;

[GlobalClass]
public partial class HitShapeDetectComponent : EcsComponent
{
    [Export] public ShapeCast3D? Node;
    [Export] public int HitsToRemove = 1;
    [Export] public float Timeout = 0;

    public List<ulong> ExcludedEntities { get; set; } = new();

    public override void _Ready()
    {
        if (Node == null)
        {
            GD.PrintErr("Hit Detect Node is null");
            return;
        }

        // Will be enabled when the entity is added
        Node.Enabled = false;
    }
}
