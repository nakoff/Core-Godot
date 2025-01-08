using LooksLike.Ecs;
using Godot;

namespace LooksLike.Framework.Components;

[GlobalClass]
public partial class DamageReceiveComponent : EcsComponent
{
    [Export] public float Value;

    public EcsEntity? Attacker;

    public DamageReceiveComponent(float value)
    {
        Value = value;
    }
}
