using Godot;

namespace LooksLike.Framework.Services;

public class RaycastResult
{
	public CollisionObject3D? Collider { get; set; }
	public Vector3 Position { get; set; }
	public Vector3 Normal { get; set; }
}

public interface IRaycastService
{
	public RaycastResult? Raycast(Vector3 from, Vector3 to);
}
