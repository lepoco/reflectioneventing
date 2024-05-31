// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing;

/// <summary>
/// Provides extension methods for the <see cref="EventBusBuilder"/> class.
/// </summary>
public static class EventBusBuilderExtensions
{
    /// <summary>
    /// Adds a consumer to the event bus builder.
    /// </summary>
    /// <typeparam name="TConsumer">The type of the consumer to add.</typeparam>
    /// <param name="builder">The event bus builder to add the consumer to.</param>
    /// <returns>The event bus builder with the consumer added.</returns>
    public static EventBusBuilder AddConsumer<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif
        TConsumer
    >(this EventBusBuilder builder)
    {
        builder.AddConsumer(typeof(TConsumer));

        return builder;
    }

    /// <summary>
    /// Adds all consumers from the specified assemblies to the event bus builder.
    /// </summary>
    /// <param name="builder">The event bus builder to add the consumers to.</param>
    /// <param name="assemblies">The assemblies to add the consumers from.</param>
    /// <returns>The event bus builder with the consumers added.</returns>
    [RequiresUnreferencedCode("Calls System.Reflection.Assembly.GetTypes()")]
    public static EventBusBuilder AddAllConsumers(
        this EventBusBuilder builder,
        params Assembly[] assemblies
    )
    {
        foreach (Assembly assembly in assemblies)
        {
            IEnumerable<Type> consumers = ExtractConsumersFromAssembly(assembly);

            RegisterAllConsumers(builder, consumers);
        }

        return builder;
    }

    [RequiresUnreferencedCode("Calls System.Reflection.Assembly.GetTypes()")]
    private static IEnumerable<Type> ExtractConsumersFromAssembly(Assembly assembly)
    {
        Type[] types = assembly.GetTypes();

        foreach (Type type in types)
        {
            Type[] typeInterfaces = type.GetInterfaces();

            if (type.IsAbstract || !type.IsClass)
            {
                continue;
            }

            foreach (Type typeInterface in typeInterfaces)
            {
                if (!typeInterface.IsGenericType)
                {
                    continue;
                }

                if (typeInterface.GetGenericTypeDefinition() == typeof(IConsumer<>))
                {
                    yield return type;
                }
            }
        }
    }

    private static void RegisterAllConsumers(EventBusBuilder builder, IEnumerable<Type> consumers)
    {
        foreach (Type consumerType in consumers)
        {
            builder.AddConsumer(consumerType);
        }
    }
}
