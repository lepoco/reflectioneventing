// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

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
        return assembly
            .GetTypes()
            .Where(t =>
                t is { IsClass: true, IsAbstract: false }
                && t.GetInterfaces()
                    .Any(i =>
                        i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>)
                    )
            );
    }

    private static void RegisterAllConsumers(EventBusBuilder builder, IEnumerable<Type> consumers)
    {
        foreach (Type consumerType in consumers)
        {
            builder.AddConsumer(consumerType);
        }
    }
}
