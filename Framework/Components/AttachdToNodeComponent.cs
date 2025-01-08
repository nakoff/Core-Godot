using LooksLike.Ecs;
using Godot;

namespace LooksLike.Framework.Components;

[GlobalClass]
public partial class AttachedToNodeComponent : EcsComponent
{
    public Node ParentNode;
    public EcsEntity ParentEntity;

    public Node ChildNode;
    public EcsEntity ChildEntity;

    public AttachedToNodeComponent(Transform3DComponent parent, Transform3DComponent child) : base()
    {
        ParentNode = parent.Node!;
        ParentEntity = parent.Entity;

        ChildNode = child.Node!;
        ChildEntity = child.Entity;
    }

    public AttachedToNodeComponent(Node parentNode, EcsEntity parentEntity, Transform3DComponent child) : base()
    {
        ParentNode = parentNode;
        ParentEntity = parentEntity;

        ChildNode = child.Node!;
        ChildEntity = child.Entity;
    }

    public AttachedToNodeComponent(Node parentNode, EcsEntity parentEntity, Node child, EcsEntity childEntity) : base()
    {
        ParentNode = parentNode;
        ParentEntity = parentEntity;

        ChildNode = child;
        ChildEntity = childEntity;
    }
}
