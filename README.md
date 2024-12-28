# Example

## Component

```csharp
[GlobalClass]
public class TestComponent : EcsComponent {}
```

## System

```csharp
public class TestSystem : SystemBase, IEntityAdded, IEntitiesUpdate
{
  [Inject] private EcsWorld _world = null!;
  [Inject] private TestService _service = null!;

  private Logger _logger = Logger.GetLogger("TestSystem", "#777FFF");

  public TestSystem() : base(new EcsFilter().With<TestComponent>())
  {
  }

  public void EntitiesAdded(List<EcsEntity> entities)
  {
    foreach (var entity in entities)
    {
      _service.PrintHello();
    }
  }

  public void EntitiesUpdate(Dictionary<ulong, EcsEntity> entities, float deltaTime)
  {
    foreach (var (_, entity) in entities)
    {
      var testComponent = entity.GetComponent<TestComponent>()!;
      _logger.Info($"Entity {entity.Id} updated");
    }
  }
}
```

## Service

```csharp
public interface ITestService
{
  void PrintHello();
}

public class TestService : ServiceBase, ITestService, IInit, IUpdate
{
  private Logger _logger = Logger.GetLogger("TestService", "#7F8888");

  public void Init()
  {
    // do something
  }

  public void Update(float deltaTime)
  {
    // do something
  }

  public void PrintHello() => _logger.Info("Hello!");
}
```

## Initialize

```csharp
public partial class MainGame : Node
{
  private readonly DIContainer _container = new();
  private EcsWorld _world = null!;
  private Logger _logger = Logger.GetLogger("MainGame", "#008f98");

  public override void _Ready()
  {
    _world = EcsWorld.Instance!;
    _container.Register(() => _world);

    // Services
    var serviceBase = new ServiceBase();
    serviceBase.Register<ITestService>(new TestService());

    // Systems
    _world.RegisterSystem(new TestSystem());

    ServiceBase.Initialize(_container);
    SystemBase.Initialize(_container);
    _world.Initialize();

    _logger.Info("Game initialized");
  }

  public override void _Process(double delta)
  {
    var dt = (float)delta;

    _world.Tick(dt);
    Service.Tick(dt);
  }

  public override void _PhysicsProcess(double delta)
  {
    _world.PhysicsTick((float)delta);
  }

  public override void _ExitTree()
  {
    _world.UnregisterAllSystems();
  }
}
```
