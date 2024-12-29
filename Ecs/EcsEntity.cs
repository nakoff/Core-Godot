using System.Collections.Generic;
using Godot;

namespace LooksLike.Ecs;

[GlobalClass]
public partial class EcsEntity : Node
{
	public ulong Id => GetInstanceId();

	private Dictionary<System.Type, Node> _components = new();

	public override void _Ready()
	{
		base._Ready();

		foreach (var item in GetChildren())
		{
			if (item is EcsComponent component)
				_components.Add(item.GetType(), component);
		}

		EcsWorld.Instance!.AddEntity(this);
	}

	public bool HasComponent<T>() where T : Node => _components.ContainsKey(typeof(T));

	public T? GetComponent<T>() where T : Node
	{
		if (HasComponent<T>())
			return (T)_components[typeof(T)];
		return null;
	}

	public Node? GetComponent(System.Type type)
	{
		if (_components.ContainsKey(type))
			return _components[type];
		return null;
	}

	public T AddComponent<T>(T component) where T : Node
	{
		if (HasComponent<T>())
			RemoveComponent<T>();

		_components.Add(component.GetType(), component);
		AddChild(component);
		return component;
	}

	public void RemoveComponent<T>() where T : Node
	{
		if (!HasComponent<T>())
			return;

		var type = typeof(T);
		_components[type].QueueFree();
		_components.Remove(type);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		EcsWorld.Instance!.RemoveEntity(this);
	}
}
