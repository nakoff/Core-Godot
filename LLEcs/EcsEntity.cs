using System.Collections.Generic;
using Godot;

namespace Core.LLEcs;

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

		EcsWorld.Instance.AddEntity(this);
	}

	public T GetComponent<T>() where T : Node
	{
		if (_components.ContainsKey(typeof(T)))
			return (T)_components[typeof(T)];
		return default;
	}

	public Node GetComponent(System.Type type)
	{
		if (_components.ContainsKey(type))
			return _components[type];
		return default;
	}

	public T AddComponent<T>() where T : Node, new()
	{
		if (_components.ContainsKey(typeof(T)))
			RemoveComponent<T>();

		var c = new T();
		_components.Add(c.GetType(), c);
		AddChild(c);
		return c;
	}

	public void RemoveComponent<T>() where T : Node
	{
		var type = typeof(T);
		if (!_components.ContainsKey(type))
			return;
		_components[type].QueueFree();
		_components.Remove(type);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		EcsWorld.Instance.RemoveEntity(this);
	}
}
