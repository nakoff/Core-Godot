using LooksLike.Ecs;
using Godot;

namespace LooksLike.Framework.Components;

[GlobalClass]
public partial class HealthComponent : EcsComponent
{
    [Export] public float Value;
    [Export] public int MaxValue;
}
