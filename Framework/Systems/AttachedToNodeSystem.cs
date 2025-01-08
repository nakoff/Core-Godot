using System.Collections.Generic;
using LooksLike.Ecs;
using LooksLike.Utils;
using Godot;
using LooksLike.Framework.Components;

namespace LooksLike.Framework.Systems;

public class AttachedToNodeSystem : SystemBase, IInitSystem, IEntitiesAdded, IComponentsRemoved
{
    private Node _entitiesRoot = null!;
    private Logger _logger = Logger.GetLogger("AttachedToNodeSystem", "#f09000");

    public AttachedToNodeSystem(Node entitiesRoot) : base(new EcsFilter()
        .With<AttachedToNodeComponent>())
    {
        _entitiesRoot = entitiesRoot;
    }

    public void Init()
    {
        _logger.IsActive = false;
    }

    public void EntitiesAdded(Dictionary<ulong, EcsEntity> entities)
    {
        foreach (var (_, entity) in entities)
        {
            var attachedToNode = entity.GetComponent<AttachedToNodeComponent>()!;
            var parentEntity = attachedToNode.ParentEntity;
            var parentNode = attachedToNode.ParentNode;
            var childNode = attachedToNode.ChildNode;
            var childEntity = attachedToNode.ChildEntity;

            childNode.GetParent()?.RemoveChild(childNode);
            parentNode.AddChild(childNode);

            if (childNode is Node3D child3D)
            {
                child3D.Transform = new Transform3D(
                    child3D.Transform.Basis,
                    Vector3.Zero
                );
            }

            _logger.Debug($"Attached parent: {parentEntity?.Name} node: {parentNode.Name} => child: {childEntity.Name}");
        }
    }

    public void ComponentsRemoved(Dictionary<ulong, EcsEntity> entities)
    {

        foreach (var (_, entity) in entities)
        {
            var attached = entity.GetRemovedComponent<AttachedToNodeComponent>();
            if (attached == null)
                continue;

            Vector3? tr = null;
            Node3D? child3D = null;
            if (attached.ChildNode is Node3D child)
            {
                child3D = child;
                tr = child3D.GlobalTransform.Origin;
            }

            attached.ParentNode.RemoveChild(attached.ChildNode);
            _entitiesRoot!.AddChild(attached.ChildNode);

            if (tr != null && child3D != null)
            {
                child3D.GlobalTransform = new Transform3D(
                    child3D.GlobalTransform.Basis,
                    tr.Value
                );
            }

            _logger.Debug($"Detached Parent: {attached.ParentEntity.Name}, child: {attached.ChildEntity.Name}");
        }
    }
}
