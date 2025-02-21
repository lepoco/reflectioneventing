// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using ReflectionEventing.Queues;
using Unity;
using Unity.Lifetime;

namespace ReflectionEventing.Unity;

/// <summary>
/// Provides extension methods for the <see cref="IUnityContainer"/> interface.
/// </summary>
public static class UnityContainerExtensions
{
    /// <summary>
    /// Adds the event bus and its related services to the specified Unity container.
    /// </summary>
    /// <param name="container">The <see cref="IUnityContainer"/> to add the event bus to.</param>
    /// <param name="configure">A delegate that configures the <see cref="UnityEventBusBuilder"/>.</param>
    /// <returns>The same Unity container so that multiple calls can be chained.</returns>
    /// <remarks>
    /// This method adds a singleton service of type <see cref="IConsumerTypesProvider"/> that uses a <see cref="HashedConsumerTypesProvider"/> with the consumers from the event bus builder.
    /// It also adds a scoped service of type <see cref="IEventBus"/> that uses the <see cref="EventBus"/> class.
    /// </remarks>
    public static IUnityContainer AddEventBus(
        this IUnityContainer container,
        Action<UnityEventBusBuilder> configure
    )
    {
        UnityEventBusBuilder builder = new(container);

        configure(builder);

        _ = container.RegisterInstance(
            builder.BuildTypesProvider(),
            new ContainerControlledLifetimeManager()
        );

        _ = container.RegisterType<IEventsQueue, EventsQueue>(
            new ContainerControlledLifetimeManager()
        );

        _ = container.RegisterType<IConsumerProvider, UnityConsumerProvider>(
            new HierarchicalLifetimeManager()
        );

        _ = container.RegisterType<IEventBus, EventBus>(new HierarchicalLifetimeManager());

        return container;
    }
}
