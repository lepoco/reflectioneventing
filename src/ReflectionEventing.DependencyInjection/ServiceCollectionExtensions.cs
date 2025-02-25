// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using ReflectionEventing.DependencyInjection.Configuration;
using ReflectionEventing.Queues;

namespace ReflectionEventing.DependencyInjection;

/// <summary>
/// Provides extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
public static class ServiceCollectionExtensions
{
#if NET8_0_OR_GREATER
    /// <summary>
    /// Adds the event bus and its related services to the specified services collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the event bus to.</param>
    /// <param name="configure">A delegate that configures the <see cref="EventBusBuilder"/>.</param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    /// <remarks>
    /// This method adds a singleton service of type <see cref="IConsumerTypesProvider"/> that uses a <see cref="HashedConsumerTypesProvider"/> with the consumers from the event bus builder.
    /// It also adds a scoped service of type <see cref="IEventBus"/> that uses the <see cref="EventBus"/> class.
    /// </remarks>
    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        Action<EventBusBuilder> configure,
        object? serviceKey
    )
    {
        DependencyInjectionEventBusBuilder builder = new(services);

        configure(builder);

        _ = services.AddKeyedSingleton(serviceKey, builder.BuildTypesProvider());
        _ = services.AddKeyedSingleton<IEventsQueue, EventsQueue>(serviceKey);
        _ = services.AddKeyedScoped<IConsumerProvider, DependencyInjectionConsumerProvider>(
            serviceKey
        );
        _ = services.AddKeyedScoped<IEventBus, DependencyInjectionEventBus>(serviceKey);

        _ = services.AddSingleton(new QueueProcessorOptionsProvider(builder.Options, serviceKey));

        if (builder.Options.UseEventsQueue)
        {
            _ = services.AddSingleton(typeof(IHostedService), builder.QueueBackgroundService);
        }

        return services;
    }
#endif

    /// <summary>
    /// Adds the event bus and its related services to the specified services collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the event bus to.</param>
    /// <param name="configure">A delegate that configures the <see cref="EventBusBuilder"/>.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    /// <remarks>
    /// This method adds a singleton service of type <see cref="IConsumerTypesProvider"/> that uses a <see cref="HashedConsumerTypesProvider"/> with the consumers from the event bus builder.
    /// It also adds a scoped service of type <see cref="IEventBus"/> that uses the <see cref="EventBus"/> class.
    /// </remarks>
    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        Action<EventBusBuilder> configure
    )
    {
        DependencyInjectionEventBusBuilder builder = new(services);

        configure(builder);

        _ = services.AddSingleton(builder.BuildTypesProvider());
        _ = services.AddSingleton<IEventsQueue, EventsQueue>();
        _ = services.AddScoped<IConsumerProvider, DependencyInjectionConsumerProvider>();
        _ = services.AddScoped<IEventBus, DependencyInjectionEventBus>();

        _ = services.AddSingleton(new QueueProcessorOptionsProvider(builder.Options));

        if (builder.Options.UseEventsQueue)
        {
            _ = services.AddSingleton(typeof(IHostedService), builder.QueueBackgroundService);
        }

        return services;
    }
}
