using Godot;

namespace LooksLike.Ecs;

[GlobalClass]
public partial class EcsComponent : Node
{
    // Will be set by EcsEntity
    public readonly EcsEntity Entity = null!;
}
