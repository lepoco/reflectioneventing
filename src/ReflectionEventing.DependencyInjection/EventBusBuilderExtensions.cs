// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing.DependencyInjection;

public static class EventBusBuilderExtensions
{
    /// <summary>
    /// Configures the event bus to use a custom background service for processing events.
    /// </summary>
    /// <typeparam name="TQueueBackgroundService">The type of the background service to use. This type must implement <see cref="IHostedService"/>.</typeparam>
    /// <returns>The current instance of <see cref="EventBusBuilder"/>.</returns>
    public static EventBusBuilder UseBackgroundService<TQueueBackgroundService>(
        this EventBusBuilder builder
    )
        where TQueueBackgroundService : class, IHostedService
    {
        if (builder is not DependencyInjectionEventBusBuilder dependencyInjectionEventBusBuilder)
        {
            throw new InvalidOperationException(
                $"The event bus builder must be of type {nameof(DependencyInjectionEventBusBuilder)} to define background service."
            );
        }

        dependencyInjectionEventBusBuilder.QueueBackgroundService = typeof(TQueueBackgroundService);

        return dependencyInjectionEventBusBuilder;
    }

    /// <summary>
    /// Adds a consumer to the event bus builder and <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="builder">The event bus builder to add the consumer to.</param>
    /// <param name="consumerType">The type of the consumer to add.</param>
    /// <returns>The event bus builder with the consumer added.</returns>
    public static EventBusBuilder AddTransientConsumer(
        this EventBusBuilder builder,
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif
        Type consumerType
    )
    {
        if (builder is not DependencyInjectionEventBusBuilder dependencyInjectionEventBusBuilder)
        {
            throw new InvalidOperationException(
                $"The event bus builder must be of type {nameof(DependencyInjectionEventBusBuilder)} to add a transient consumer."
            );
        }

        return dependencyInjectionEventBusBuilder.AddConsumer(
            consumerType,
            ServiceLifetime.Transient
        );
    }

    /// <summary>
    /// Adds a consumer to the event bus builder and <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TConsumer">The type of the consumer to add.</typeparam>
    /// <param name="builder">The event bus builder to add the consumer to.</param>
    /// <returns>The event bus builder with the consumer added.</returns>
    public static EventBusBuilder AddTransientConsumer<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif
        TConsumer
    >(this EventBusBuilder builder)
    {
        if (builder is not DependencyInjectionEventBusBuilder dependencyInjectionEventBusBuilder)
        {
            throw new InvalidOperationException(
                $"The event bus builder must be of type {nameof(DependencyInjectionEventBusBuilder)} to add a transient consumer."
            );
        }

        return dependencyInjectionEventBusBuilder.AddConsumer(
            typeof(TConsumer),
            ServiceLifetime.Transient
        );
    }

    /// <summary>
    /// Adds a consumer to the event bus builder and <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="builder">The event bus builder to add the consumer to.</param>
    /// <param name="consumerType">The type of the consumer to add.</param>
    /// <returns>The event bus builder with the consumer added.</returns>
    public static EventBusBuilder AddScopedConsumer(
        this EventBusBuilder builder,
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif
        Type consumerType
    )
    {
        if (builder is not DependencyInjectionEventBusBuilder dependencyInjectionEventBusBuilder)
        {
            throw new InvalidOperationException(
                $"The event bus builder must be of type {nameof(DependencyInjectionEventBusBuilder)} to add a scoped consumer."
            );
        }

        return dependencyInjectionEventBusBuilder.AddConsumer(consumerType, ServiceLifetime.Scoped);
    }

    /// <summary>
    /// Adds a consumer to the event bus builder and <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TConsumer">The type of the consumer to add.</typeparam>
    /// <param name="builder">The event bus builder to add the consumer to.</param>
    /// <returns>The event bus builder with the consumer added.</returns>
    public static EventBusBuilder AddScopedConsumer<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif
        TConsumer
    >(this EventBusBuilder builder)
    {
        if (builder is not DependencyInjectionEventBusBuilder dependencyInjectionEventBusBuilder)
        {
            throw new InvalidOperationException(
                $"The event bus builder must be of type {nameof(DependencyInjectionEventBusBuilder)} to add a scoped consumer."
            );
        }

        return dependencyInjectionEventBusBuilder.AddConsumer(
            typeof(TConsumer),
            ServiceLifetime.Scoped
        );
    }

    /// <summary>
    /// Adds a consumer to the event bus builder and <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="builder">The event bus builder to add the consumer to.</param>
    /// <param name="consumerType">The type of the consumer to add.</param>
    /// <returns>The event bus builder with the consumer added.</returns>
    public static EventBusBuilder AddSingletonConsumer(
        this EventBusBuilder builder,
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif
        Type consumerType
    )
    {
        if (builder is not DependencyInjectionEventBusBuilder dependencyInjectionEventBusBuilder)
        {
            throw new InvalidOperationException(
                $"The event bus builder must be of type {nameof(DependencyInjectionEventBusBuilder)} to add a singleton consumer."
            );
        }

        return dependencyInjectionEventBusBuilder.AddConsumer(
            consumerType,
            ServiceLifetime.Singleton
        );
    }

    /// <summary>
    /// Adds a consumer to the event bus builder and <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TConsumer">The type of the consumer to add.</typeparam>
    /// <param name="builder">The event bus builder to add the consumer to.</param>
    /// <returns>The event bus builder with the consumer added.</returns>
    public static EventBusBuilder AddSingletonConsumer<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif
        TConsumer
    >(this EventBusBuilder builder)
    {
        if (builder is not DependencyInjectionEventBusBuilder dependencyInjectionEventBusBuilder)
        {
            throw new InvalidOperationException(
                $"The event bus builder must be of type {nameof(DependencyInjectionEventBusBuilder)} to add a singleton consumer."
            );
        }

        return dependencyInjectionEventBusBuilder.AddConsumer(
            typeof(TConsumer),
            ServiceLifetime.Singleton
        );
    }

    /// <summary>
    /// Adds a consumer to the event bus builder and <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TConsumer">The type of the consumer to add.</typeparam>
    /// <param name="builder">The event bus builder to add the consumer to.</param>
    /// <param name="lifetime">The service lifetime of the consumer.</param>
    /// <returns>The event bus builder with the consumer added.</returns>
    public static EventBusBuilder AddConsumer<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif
        TConsumer
    >(this EventBusBuilder builder, ServiceLifetime lifetime)
    {
        if (builder is not DependencyInjectionEventBusBuilder dependencyInjectionEventBusBuilder)
        {
            throw new InvalidOperationException(
                $"The event bus builder must be of type {nameof(DependencyInjectionEventBusBuilder)} to add a transient consumer."
            );
        }

        return dependencyInjectionEventBusBuilder.AddConsumer(typeof(TConsumer), lifetime);
    }
}
