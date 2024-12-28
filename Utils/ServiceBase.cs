using System;
using System.Collections.Generic;
using Core.DependencyInjection;
using Godot;

namespace Core.Services;

public class ServiceBase : Service
{
    protected static DIContainer? Container { get; private set; }
    private static Dictionary<Type, Service> _services = new();

    public static void Initialize(DIContainer container)
    {
        Container = container;
        foreach (var service in _services)
        {
            var type = service.Key;
            var value = service.Value;
            if (type is null || value is null)
                continue;

            container.Register(type, () => value);
        }

        Service.Initialize();
    }

    public void Register<TService>(TService service) where TService : class
    {
        if (service is Service serviceBase)
        {
            Service.Register(serviceBase);
            _services[typeof(TService)] = serviceBase;
            return;
        }

        GD.PrintErr($"{typeof(TService)} is not a Service.");
    }
}
