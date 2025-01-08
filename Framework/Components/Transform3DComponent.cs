using LooksLike.Ecs;
using Godot;

namespace LooksLike.Framework.Components;

[GlobalClass]
public partial class Transform3DComponent : EcsComponent
{
    [Export] public Node3D? Node { get; set; }

    public Transform3D Transform3D
    {
        get => Node?.GlobalTransform ?? Transform3D.Identity;
        set
        {
            if (Node != null)
                Node.GlobalTransform = value;
        }
    }

    public Transform3D LocalTransform3D
    {
        get => Node?.Transform ?? Transform3D.Identity;
        set
        {
            if (Node != null)
                Node.Transform = value;
        }
    }

    public Vector3 Scale
    {
        get => Node?.Scale ?? Vector3.One;
        set
        {
            if (Node != null)
                Node.Scale = value;
        }
    }
}
