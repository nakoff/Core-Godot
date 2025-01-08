using LooksLike.Ecs;
using Godot;

namespace LooksLike.Framework.Components;

[GlobalClass]
public partial class MoveComponent : EcsComponent
{
	[Export] public float X;
	[Export] public float Y;
	[Export] public float Speed = 2.0f;
	[Export] public float JumpVelocity = 4.5f;
	[Export] public Vector3 Direction = new Vector3(0, 0, 0);
	[Export] public bool IsJumping = false;

	public Vector3 PrevPosition = new Vector3(0, 0, 0);
}
