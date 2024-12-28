using System.Collections.Generic;

namespace LooksLike.Services;

public interface IInit
{
    void Init();
}

public interface IUpdate
{
    void Update(float dt);
}

public class Service
{
    public static Service Instance => _instance ??= new Service();
    private static Service? _instance;

    private List<Service> _services = new();
    private List<IInit> _inits = new();
    private List<IUpdate> _updates = new();

    public static void Register(Service service)
    {
        Instance._services.Add(service);

        if (service is IInit init)
            Instance._inits.Add(init);

        if (service is IUpdate update)
            Instance._updates.Add(update);
    }

    public static T? GetService<T>() where T : Service
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
