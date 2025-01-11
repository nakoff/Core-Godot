using LooksLike.Ecs;
using Godot;

namespace LooksLike.Framework.Components;

[GlobalClass]
public partial class UINodeComponent : EcsComponent
{
    [Export] public Control? Node { get; set; }
}
