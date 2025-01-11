using LooksLike.Ecs;
using Godot;

namespace LooksLike.Framework.Components;

[GlobalClass]
public partial class Transform2DComponent : EcsComponent
{
    [Export] public Node2D? Node { get; set; }

    public Transform2D Transform2D
    {
        get => Node?.GlobalTransform ?? new Transform2D();
        set
        {
            if (Node != null)
                Node.GlobalTransform = value;
        }
    }

    public Transform2D LocalTransform2D
    {
        get => Node?.Transform ?? new Transform2D();
        set
        {
            if (Node != null)
                Node.Transform = value;
        }
    }

    public Vector2 Scale
    {
        get => Node?.Scale ?? Vector2.One;
        set
        {
            if (Node != null)
                Node.Scale = value;
        }
    }
}
