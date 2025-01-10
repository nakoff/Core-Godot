using System.Collections.Generic;
using LooksLike.Utils;
using Godot;

namespace LooksLike.Ecs;

[GlobalClass]
public partial class EcsWorld : Node
{
	public static EcsWorld? Instance => _instance;

	[Export] private Godot.Collections.Array<EcsEntity> entities = new();

	private static EcsWorld? _instance;
	private List<IInitSystem> _initableSystems = new();
	private List<IDestroySystem> _destroyableSystems = new();

	private List<EcsSystem> _allSystems = new();
	private List<IEntitiesAdded> _entitiesAddedSystems = new();
	private List<IComponentsRemoved> _entitiesRemoveSystems = new();
	private List<IEntitiesUpdate> _entitiesUpdateSystems = new();
	private List<IEntitiesPhysicsUpdate> _entitiesPhysicsUpdateSystems = new();

	private List<EcsEntity> _entities = new();
	private List<EcsEntity> _removedEntities = new();
	private List<EcsFilter> _filters = new();
	private Dictionary<uint, Dictionary<ulong, EcsEntity>> _filteredEntities = new();
	private Dictionary<uint, Dictionary<ulong, EcsEntity>> _addedEntities = new();
	private Dictionary<uint, Dictionary<ulong, EcsEntity>> _removedEntityComponents = new();

	private List<EcsEntity> _allRemovedEntityComponents = new();
	private List<EcsEntity> _allAddedEntities = new();

	private Logger _logger = Logger.GetLogger("LooksLike/Ecs", "#ff00ff");

	private ulong _lastUpdateEntities = 0;

	public override void _EnterTree()
	{
		if (_instance == null)
			_instance = this;
		else
			QueueFree();
	}

	public void Initialize()
	{
		if (Instance == null)
		{
			_logger.Error("EcsWorld is null");
			return;
		}

		foreach (var system in _initableSystems)
			system.Init();

		foreach (var system in _allSystems)
		{
			if (!_filters.Contains(system.Filter))
				_filters.Add(system.Filter);
		}

		foreach (var filter in _filters)
		{
			if (!_filteredEntities.ContainsKey(filter.Id))
				_filteredEntities.Add(filter.Id, new());

			UpdateFilteredEntities(filter);
		}
	}

	public void Tick(float dt)
	{
		var currentTime = Time.GetTicksMsec();
		if (_lastUpdateEntities != currentTime)
		{
			_lastUpdateEntities = currentTime;
			foreach (var f in _filters)
				UpdateFilteredEntities(f);
		}

		// removed
		foreach (var system in _entitiesRemoveSystems)
		{
			var filterId = system.Filter.Id;
			if (_removedEntityComponents.ContainsKey(filterId))
			{
				var entities = _removedEntityComponents[filterId];
				system.ComponentsRemoved(entities);
			}
		}
		_removedEntityComponents.Clear();

		// added
		foreach (var system in _entitiesAddedSystems)
		{
			var filterId = system.Filter.Id;
			if (_addedEntities.ContainsKey(filterId) && _addedEntities[filterId].Count > 0)
			{
				var entities = _addedEntities[filterId];
				system.EntitiesAdded(entities);
			}
		}
		_addedEntities.Clear();

		// update
		foreach (var system in _entitiesUpdateSystems)
		{
			var filterId = system.Filter.Id;
			if (_filteredEntities[filterId].Count > 0)
				system.EntitiesUpdate(_filteredEntities[filterId], dt);
		}

		// clear
		if (_allRemovedEntityComponents.Count > 0)
		{
			foreach (var entity in _allRemovedEntityComponents)
			{
				foreach (var component in entity.RemovedComponents.Values)
					component.QueueFree();

				entity.RemovedComponents.Clear();
			}
			_allRemovedEntityComponents.Clear();
		}

		if (_allAddedEntities.Count > 0)
		{
			foreach (var entity in _allAddedEntities)
				entity.AddedComponents.Clear();
			_allAddedEntities.Clear();
		}

		if (_removedEntities.Count > 0)
		{
			List<EcsEntity> lateRemove = new();
			foreach (var entity in _removedEntities)
			{
				if (entity.Components.Count > 0)
				{
					lateRemove.Add(entity);
					foreach (var component in entity.Components)
						entity.RemoveComponent(component);
				}
				else
				{
					_entities.Remove(entity);
					entities.Remove(entity);
					entity.QueueFree();
				}
			}
			_removedEntities = lateRemove;
		}
	}

	public void PhysicsTick(float dt)
	{
		var currentTime = Time.GetTicksMsec();
		if (_lastUpdateEntities != currentTime)
		{
			_lastUpdateEntities = currentTime;
			foreach (var f in _filters)
				UpdateFilteredEntities(f);
		}

		// physics update
		foreach (var system in _entitiesPhysicsUpdateSystems)
		{
			var filterId = system.Filter.Id;
			if (_filteredEntities.ContainsKey(filterId) && _filteredEntities[filterId].Count > 0)
				system.EntitiesPhysicsUpdate(_filteredEntities[filterId], dt);
		}
	}

	public EcsEntity CreateEntity(string name, Node? parent = null)
	{
		var p = parent != null ? parent : GetTree().Root.GetChild(0);
		var entity = new EcsEntity();
		entity.Name = name;
		p.AddChild(entity);

		return entity;
	}

	public EcsEntity AddEntity(EcsEntity entity)
	{
		_entities.Add(entity);
		entities.Add(entity);
		return entity;
	}

	public void RemoveEntity(EcsEntity entity)
	{
		if (!_entities.Contains(entity) || entity.MarkToRemove)
			return;

		// entity.MarkToRemove = true;
		ReflectionHelper.SetReadonlyField(entity, "MarkToRemove", true);
		entity.ProcessMode = ProcessModeEnum.Disabled;

		if (!_removedEntities.Contains(entity))
			_removedEntities.Add(entity);
	}

	public Dictionary<ulong, EcsEntity> GetFilteredEntities(EcsFilter filter)
	{
		if (!_filteredEntities.ContainsKey(filter.Id))
			_filteredEntities.Add(filter.Id, new());

		var math = false;
		foreach (var c in filter.WithComponents)
		{
			foreach (var f in _filters)
			{
				if (!f.WithComponents.Contains(c) || f.WithoutComponents.Contains(c))
				{
					math = false;
					break;
				}

				math = true;
			}
		}

		if (!math)
			UpdateFilteredEntities(filter);

		return _filteredEntities[filter.Id];
	}

	public void RegisterSystem(IEcsSystem system)
	{
		var ecsSystem = (EcsSystem)system;
		_allSystems.Add(ecsSystem);

		if (system is IInitSystem initSystem) _initableSystems.Add(initSystem);
		if (system is IDestroySystem destroySystem) _destroyableSystems.Add(destroySystem);

		if (system is IEntitiesAdded entityAdded) _entitiesAddedSystems.Add(entityAdded);
		if (system is IEntitiesUpdate entitiesUpdate) _entitiesUpdateSystems.Add(entitiesUpdate);
		if (system is IEntitiesPhysicsUpdate entitiesPhysicsUpdate) _entitiesPhysicsUpdateSystems.Add(entitiesPhysicsUpdate);
		if (system is IComponentsRemoved entitiesRemoved) _entitiesRemoveSystems.Add(entitiesRemoved);
	}

	public void UnregisterAllSystems()
	{
		foreach (var system in _destroyableSystems)
			system.Destroy();

		_allSystems.Clear();
		_initableSystems.Clear();
		_destroyableSystems.Clear();
		_entitiesUpdateSystems.Clear();
		_entitiesPhysicsUpdateSystems.Clear();
		_entitiesAddedSystems.Clear();
		_entitiesRemoveSystems.Clear();
	}

	private void UpdateFilteredEntities(EcsFilter filter)
	{
		_filteredEntities[filter.Id].Clear();
		foreach (var entity in _entities)
		{
			bool? match = null;
			var matchRemoved = false;
			var matchAdded = false;

			foreach (var componentType in filter.WithComponents)
			{
				var hasComponent = entity.HasComponent(componentType);
				if (match == null || match == true)
					match = hasComponent;

				if (entity.AddedComponents.Count > 0 && !matchAdded)
					matchAdded = entity.AddedComponents.ContainsKey(componentType);

				if (match == false && !hasComponent && entity.RemovedComponents.Count > 0)
					matchRemoved = entity.RemovedComponents.ContainsKey(componentType);

				if (match == false && matchRemoved == false)
					break;
			}

			if (match == true || matchRemoved == true)
			{
				foreach (var componentType in filter.WithoutComponents)
				{
					if (match == true && entity.HasComponent(componentType))
					{
						match = false;
						matchRemoved = false;
					}

					if (matchRemoved == true)
						matchRemoved = !entity.RemovedComponents.ContainsKey(componentType);

					if (match == false && matchRemoved == false)
						break;
				}
			}

			if (match == true)
			{
				if (!_filteredEntities[filter.Id].ContainsKey(entity.Id))
					_filteredEntities[filter.Id].Add(entity.Id, entity);

				// check added
				if (matchAdded)
				{
					if (!_addedEntities.ContainsKey(filter.Id))
						_addedEntities.Add(filter.Id, new());

					if (!_addedEntities[filter.Id].ContainsKey(entity.Id))
						_addedEntities[filter.Id].Add(entity.Id, entity);
				}
			}

			// check removed
			if (matchRemoved)
			{
				if (!_removedEntityComponents.ContainsKey(filter.Id))
					_removedEntityComponents.Add(filter.Id, new());

				// new list from dict values
				if (!_removedEntityComponents[filter.Id].ContainsKey(entity.Id))
					_removedEntityComponents[filter.Id].Add(entity.Id, entity);
			}

			if (entity.RemovedComponents.Count > 0)
				_allRemovedEntityComponents.Add(entity);

			if (entity.AddedComponents.Count > 0)
				_allAddedEntities.Add(entity);
		}
	}
}
