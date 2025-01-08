using System.Collections.Generic;
using LooksLike.Ecs;
using Godot;

namespace LooksLike.Framework.Components;

[GlobalClass]
public partial class TriggerDetectComponent : EcsComponent
{
    [Export] public Area3D? Node;
    [Export] public int HitsToRemove = 1;
    [Export] public float Timeout = 0;

    public Node3D? DetectedBody { get; set; }
    public List<ulong> ExcludedEntities { get; set; } = new();

    public override void _Ready()
    {
        if (Node == null)
        {
            GD.PrintErr("Collision Detect Node is null");
            return;
        }

        Node.BodyEntered += OnBodyEntered;
        Node.BodyExited += OnBodyExited;
    }

    public void OnBodyEntered(Node3D body)
    {
        if (Timeout > 0)
            return;

        if (body?.Owner is EcsEntity otherEntity)
        {
            if (otherEntity.Id == Owner.GetInstanceId())
                return;

            if (ExcludedEntities.Contains(otherEntity.Id))
                return;
        }

        DetectedBody = body;
    }
    public void OnBodyExited(Node3D body)
    {
        if (body == DetectedBody)
            DetectedBody = null;
    }
}
