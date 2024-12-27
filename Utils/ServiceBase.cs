using Core.DependencyInjection;
using Godot;

namespace Core.Services;

public class ServiceBase : Service
{
    public void Register<TService>(DIContainer container, TService service) where TService : class
    {
        if (service is Service serviceBase)
        {
            container.Register<TService>(() => (service));
            Service.Register(serviceBase);
            return;
        }

        GD.PrintErr($"{typeof(TService)} is not a Service.");
    }
}
