using System;
using System.Collections.Generic;
using System.Reflection;

namespace LooksLike.DependencyInjection;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class InjectAttribute : Attribute { }

public class DIContainer
{
	private readonly Dictionary<Type, Func<object>> _registrations = new();

	public void Register<TService>(Func<TService> implementationFactory) where TService : class
		=> _registrations[typeof(TService)] = () => implementationFactory();

	public void Register(Type serviceType, Func<object> implementationFactory)
		=> _registrations[serviceType] = implementationFactory;

	public void InjectDependencies(object target)
	{
		if (target == null) throw new ArgumentNullException(nameof(target));

		var targetType = target.GetType();
		var members = targetType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		foreach (var member in members)
		{
			var injectAttribute = member.GetCustomAttribute<InjectAttribute>();
			if (injectAttribute == null) continue;

			Type? memberType = null;
			if (member is PropertyInfo property)
				memberType = property.PropertyType;
			else if (member is FieldInfo fieldInfo)
				memberType = fieldInfo.FieldType;

			if (memberType == null)
				throw new InvalidOperationException($"Unsupported member type on {member.Name}.");

			var dependency = Resolve(memberType);

			if (member is PropertyInfo propertyInfo && propertyInfo.CanWrite)
				propertyInfo.SetValue(target, dependency);
			else if (member is FieldInfo fieldMember)
				fieldMember.SetValue(target, dependency);
			else
				throw new InvalidOperationException($"Cannot inject dependency into {member.Name}.");
		}
	}

	private TService Resolve<TService>() where TService : class
		=> (TService)Resolve(typeof(TService));

	private object Resolve(Type serviceType)
	{
		if (_registrations.TryGetValue(serviceType, out var factory))
		{
			var instance = factory();
			InjectDependencies(instance);
			return instance;
		}

		throw new InvalidOperationException($"Service of type {serviceType} is not registered.");
	}
}
