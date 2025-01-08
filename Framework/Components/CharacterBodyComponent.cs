using LooksLike.Ecs;
using Godot;

namespace LooksLike.Framework.Components;

[GlobalClass]
public partial class CharacterBodyComponent : EcsComponent
{
    [Export] public CharacterBody3D? Node { get; set; }
    [Export] public Node3D? Visual { get; set; }
    [Export] public float Inertia { get; set; } = 0.05f;

    public override void _Ready()
    {
        if (Node == null)
            GD.PrintErr("Node is null");
    }
}
