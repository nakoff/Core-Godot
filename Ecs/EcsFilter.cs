using System.Collections.Generic;

namespace LooksLike.Ecs;

public class EcsFilter
{
    private static uint _idIncremented = 0;

    public readonly uint Id;
    public readonly HashSet<System.Type> WithComponents = new();
    public readonly HashSet<System.Type> WithoutComponents = new();

    public EcsFilter()
    {
        Id = ++_idIncremented;
    }

    public EcsFilter With<T>() where T : EcsComponent
    {
        WithComponents.Add(typeof(T));
        return this;
    }

    public EcsFilter Without<T>() where T : EcsComponent
    {
        WithoutComponents.Add(typeof(T));
        return this;
    }
}
