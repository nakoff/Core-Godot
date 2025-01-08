using LooksLike.Ecs;
using Godot;

namespace LooksLike.Framework.Components;

[GlobalClass]
public partial class AreaNodeComponent : EcsComponent
{
    [Export] public Area3D? Node;


    public override void _Ready()
    {
        if (Node == null)
            GD.PrintErr("Area Node is null");
    }
}
