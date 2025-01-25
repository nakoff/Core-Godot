using System.Collections.Generic;
using Godot;

namespace LooksLike.Ecs;

[GlobalClass]
public partial class EcsEntity : Node
{
	public ulong Id => GetInstanceId();
	public readonly bool MarkToRemove;

	public List<EcsComponent> Components => new(_components.Values);
	public readonly Dictionary<System.Type, EcsComponent> AddedComponents = new();
	public readonly Dictionary<System.Type, EcsComponent> RemovedComponents = new();

	private readonly Dictionary<System.Type, EcsComponent> _components = new();

	public override void _Ready()
	{
		base._Ready();

		foreach (var item in GetChildren())
		{
			if (item is EcsComponent component)
				AddComponent(component, false);
		}

		EcsWorld.Instance!.AddEntity(this);
	}

	public bool HasComponent<T>() where T : EcsComponent => HasComponent(typeof(T));
	public bool HasComponent(System.Type type) => _components.ContainsKey(type);

	public T? GetComponent<T>() where T : EcsComponent => (T?)GetComponent(typeof(T));
	public EcsComponent? GetComponent(System.Type type) => _components.ContainsKey(type) ? _components[type] : null;

	public bool HasRemovedComponent<T>() where T : EcsComponent => HasRemovedComponent(typeof(T));
	public bool HasRemovedComponent(System.Type type) => RemovedComponents.ContainsKey(type);

	public T? GetRemovedComponent<T>() where T : EcsComponent => (T?)GetRemovedComponent(typeof(T));
	public EcsComponent? GetRemovedComponent(System.Type type) => RemovedComponents.ContainsKey(type) ? RemovedComponents[type] : null;

	public T AddComponent<T>(T component, bool attach = true) where T : EcsComponent
	{
		if (HasComponent<T>())
			RemoveComponent<T>();

		var type = component.GetType();

		// set readonly field: component.Entity = this;
		ReflectionHelper.SetReadonlyField(component, "Entity", this);

		_components.Add(type, component);

		if (!AddedComponents.ContainsKey(type))
			AddedComponents.Add(type, component);

		if (attach)
			AddChild(component);

		return component;
	}

	public void RemoveComponent<T>() where T : EcsComponent => RemoveComponent(typeof(T));
	public void RemoveComponent(EcsComponent component) => RemoveComponent(component.GetType());
	public void RemoveComponent(System.Type type)
	{
		var component = GetComponent(type);
		AddedComponents.Remove(type);
		if (component == null)
			return;

		if (!RemovedComponents.ContainsKey(type))
			RemovedComponents.Add(type, component);

		_components.Remove(type);

		// _components[type].QueueFree(); // will be removed by EcsWorld
		if (component.GetParent() != null)
			component.GetParent().RemoveChild(component);
	}


	public override void _ExitTree()
	{
		base._ExitTree();
		EcsWorld.Instance!.RemoveEntity(this);
	}
}
