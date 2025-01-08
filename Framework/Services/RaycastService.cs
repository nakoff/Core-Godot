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

	public CollisionObject3D? Raycast(Vector3 from, Vector3 to)
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

		if (result.ContainsKey("collider"))
			return result["collider"].As<CollisionObject3D>();

		return null;
	}
}
