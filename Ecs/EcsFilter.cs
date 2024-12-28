using System.Collections.Generic;
using System;
using Godot;

namespace LooksLike.Ecs;

public class EcsFilter
{
    public readonly string Id;
    public readonly HashSet<System.Type> WithComponents = new();
    public readonly HashSet<System.Type> WithoutComponents = new();

    public EcsFilter()
    {
        Id = Guid.NewGuid().ToString();
    }

    public EcsFilter With<T>() where T : Node
    {
        WithComponents.Add(typeof(T));
        return this;
    }

    public EcsFilter Without<T>() where T : Node
    {
        WithoutComponents.Add(typeof(T));
        return this;
    }
}
