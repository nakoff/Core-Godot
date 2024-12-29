using System.Collections.Generic;
using LooksLike.Utils;
using Godot;

namespace LooksLike.Ecs;

public partial class EcsWorld : Node
{
    public static EcsWorld? Instance => _instance;

    [Export] private Godot.Collections.Array<EcsEntity> entities = new();

    private static EcsWorld? _instance;
    private List<EcsSystem> _allSystems = new();
    private Dictionary<int, IEntityAdded> _initableSystems = new();
    private Dictionary<int, IEntityRemoved> _removableSystems = new();
    private Dictionary<int, IEntitiesUpdate> _updatableSystems = new();
    private Dictionary<int, IEntitiesPhysicsUpdate> _physicsUpdatableSystems = new();

    private List<EcsEntity> _entities = new();
    private List<EcsFilter> _filters = new();
    private Dictionary<string, Dictionary<ulong, EcsEntity>> _filteredEntities = new();
    private Dictionary<string, List<EcsEntity>> _addedEntities = new();
    private Dictionary<string, List<ulong>> _removedEntities = new();

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

        foreach (var system in _allSystems)
        {
            if (_filters.Contains(system.Filter))
                continue;

            _filters.Add(system.Filter);
            UpdateFilteredEntities(system.Filter);
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

        foreach (var system in _allSystems)
        {
            var filteredEntities = _filteredEntities[system.Filter.Id];

            // added
            if (_initableSystems.ContainsKey(system.Id)
                    && _addedEntities.ContainsKey(system.Filter.Id)
                    && _addedEntities[system.Filter.Id].Count > 0)
            {
                var entities = _addedEntities[system.Filter.Id];
                _initableSystems[system.Id].EntitiesAdded(entities);
            }

            // update
            if (system is IEntitiesUpdate updateSystem)
                updateSystem.EntitiesUpdate(filteredEntities, dt);

            // physics update
            if (system is IEntitiesPhysicsUpdate physicsUpdateSystem)
                physicsUpdateSystem.EntitiesPhysicsUpdate(filteredEntities, dt);
        }

        _addedEntities.Clear();
        _removedEntities.Clear();
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

        foreach (var system in _allSystems)
        {
            var filteredEntities = _filteredEntities[system.Filter.Id];

            // physics update
            if (system is IEntitiesPhysicsUpdate physicsUpdateSystem)
                physicsUpdateSystem.EntitiesPhysicsUpdate(filteredEntities, dt);
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
        _entities.Remove(entity);
        entities.Remove(entity);
        entity.QueueFree();
    }

    public Dictionary<ulong, EcsEntity> GetFilteredEntities(EcsFilter filter)
    {
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
        if (system is IEntityAdded entityAdded) _initableSystems.Add(ecsSystem.Id, entityAdded);
        if (system is IEntitiesUpdate entitiesUpdate) _updatableSystems.Add(ecsSystem.Id, entitiesUpdate);
        if (system is IEntitiesPhysicsUpdate entitiesPhysicsUpdate) _physicsUpdatableSystems.Add(ecsSystem.Id, entitiesPhysicsUpdate);
        if (system is IEntityRemoved entitiesRemoved) _removableSystems.Add(ecsSystem.Id, entitiesRemoved);
    }

    public void UnregisterAllSystems()
    {
        _allSystems.Clear();
        _updatableSystems.Clear();
        _physicsUpdatableSystems.Clear();
        _initableSystems.Clear();
        _removableSystems.Clear();
    }

    private void UpdateFilteredEntities(EcsFilter filter)
    {
        if (!_filteredEntities.ContainsKey(filter.Id))
            _filteredEntities.Add(filter.Id, new());

        Dictionary<ulong, EcsEntity> newFilteredEntities = new();
        foreach (var entity in _entities)
        {
            var match = false;
            foreach (var c in filter.WithComponents)
            {
                match = false;

                if (newFilteredEntities.ContainsKey(entity.Id))
                    break;

                if (entity.GetComponent(c) == null)
                    break;

                match = true;
            }

            if (match)
            {
                foreach (var c in filter.WithoutComponents)
                {
                    if (entity.GetComponent(c) != null)
                    {
                        match = false;
                        break;
                    }
                }
            }

            if (match)
                newFilteredEntities.Add(entity.Id, entity);
        }

        foreach (var c in filter.WithoutComponents)
        {
        }

        // check added entityIds
        List<EcsEntity> addedEntities = new();
        var oldFilteredEntities = _filteredEntities[filter.Id];
        foreach (var (id, entity) in newFilteredEntities)
        {
            if (!oldFilteredEntities.ContainsKey(id))
                addedEntities.Add(entity);
        }

        if (addedEntities.Count > 0)
        {
            if (!_addedEntities.ContainsKey(filter.Id))
                _addedEntities.Add(filter.Id, new List<EcsEntity>());
            _addedEntities[filter.Id] = addedEntities;
        }

        _filteredEntities[filter.Id] = newFilteredEntities;
    }
}
