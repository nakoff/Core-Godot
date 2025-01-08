using Godot;

namespace LooksLike.Framework.Services;

public interface IRaycastService
{
	public CollisionObject3D? Raycast(Vector3 from, Vector3 to);
}
