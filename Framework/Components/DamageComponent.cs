using LooksLike.Ecs;
using Godot;

namespace LooksLike.Framework.Components;

[GlobalClass]
public partial class DamageComponent : EcsComponent
{
    [Export] public float Value;
}
