using LooksLike.Utils;
using Godot;

namespace LooksLike.Framework.Services;

public class RaycastService : ServiceBase, IRaycastService
{
	private Node3D _root = null!;
	private Logger _logger = Logger.GetLogger("RaycastService", "#ccc05a");

	public RaycastService(Node3D root)
	{
		_root = root;
	}

	public RaycastResult? Raycast(Vector3 from, Vector3 to)
	{
		var query = new PhysicsRayQueryParameters3D
		{
			From = @from,
			To = to,
			CollisionMask = uint.MaxValue, // Check all layers
		};

		var spaceState = _root.GetWorld3D().DirectSpaceState;
		var result = spaceState.IntersectRay(query);
		if (result.Count == 0)
			return null;

		return new RaycastResult
		{
			Collider = result.ContainsKey("collider") ? result["collider"].As<CollisionObject3D>() : null,
			Position = result.ContainsKey("position") ? result["position"].As<Vector3>() : Vector3.Zero,
			Normal = result.ContainsKey("normal") ? result["normal"].As<Vector3>() : Vector3.Zero,
		};

	}
}
