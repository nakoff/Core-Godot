using LooksLike.Ecs;
using Godot;

namespace LooksLike.Framework.Components;

[GlobalClass]
public partial class LifeTimeComponent : EcsComponent
{
    [Export] public float ValueSeconds;
}
