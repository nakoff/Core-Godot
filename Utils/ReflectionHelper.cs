
using System;
using System.Reflection;

public static class ReflectionHelper
{
    public static void SetReadonlyField<T>(T target, string fieldName, object value)
    {
        if (target == null)
            throw new ArgumentNullException(nameof(target));

        FieldInfo? field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (field == null)
            throw new ArgumentException($"Field '{fieldName}' not found on type '{target.GetType()}'.");

        if (!field.IsInitOnly)
            throw new InvalidOperationException($"Field '{fieldName}' is not readonly.");

        field.SetValue(target, value);
    }
}
