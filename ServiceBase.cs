
using System.Collections.Generic;

namespace Core.Services;

public interface IInit
{
    void Init();
}

public interface IUpdate
{
    void Update(float dt);
}

public class ServiceBase
{
    public static ServiceBase Instance => _instance ??= new ServiceBase();
    private static ServiceBase _instance;

    private List<ServiceBase> _services = new();
    private List<IInit> _inits = new();
    private List<IUpdate> _updates = new();

    public static void Register(ServiceBase service)
    {
        Instance._services.Add(service);

        if (service is IInit init)
            Instance._inits.Add(init);

        if (service is IUpdate update)
            Instance._updates.Add(update);
    }

    public static T GetService<T>() where T : ServiceBase
    {
        return Instance._services.Find(x => x is T) as T;
    }

    public static void Initialize()
    {
        foreach (var init in Instance._inits)
            init.Init();
    }

    public static void Tick(float dt)
    {
        foreach (var update in Instance._updates)
            update.Update(dt);
    }
}
