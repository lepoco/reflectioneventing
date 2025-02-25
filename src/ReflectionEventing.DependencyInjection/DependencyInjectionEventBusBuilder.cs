// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using ReflectionEventing.DependencyInjection.Services;

namespace ReflectionEventing.DependencyInjection;

/// <summary>
/// Represents a builder for configuring the event bus with .NET Core's built-in dependency injection.
/// </summary>
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class DependencyInjectionEventBusBuilder(IServiceCollection services) : EventBusBuilder
{
    internal Type QueueBackgroundService { get; set; } = typeof(DependencyInjectionQueueProcessor);

    /// <summary>
    /// Adds a consumer to the event bus and <see cref="IServiceCollection"/> with a specified service lifetime.
    /// </summary>
    /// <param name="consumerType">The type of the consumer to add.</param>
    /// <param name="lifetime">The service lifetime of the consumer.</param>
    /// <returns>The current instance of <see cref="EventBusBuilder"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the consumer is already registered with a different lifetime or if the consumer is not registered in the service collection.
    /// </exception>
    public virtual EventBusBuilder AddConsumer(
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif
        Type consumerType,
        ServiceLifetime lifetime
    )
    {
        ServiceDescriptor? descriptor = services.FirstOrDefault(d => d.ServiceType == consumerType);

        if (descriptor is not null)
        {
            if (descriptor.Lifetime != lifetime)
            {
                throw new InvalidOperationException(
                    "Event consumer must be registered with the same lifetime as the one provided."
                );
            }

            return base.AddConsumer(consumerType);
        }

        services.Add(new ServiceDescriptor(consumerType, consumerType, lifetime));

        return base.AddConsumer(consumerType);
    }

    /// <inheritdoc />
    public override EventBusBuilder AddConsumer(
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif
        Type consumerType
    )
    {
        ServiceDescriptor? descriptor = services.FirstOrDefault(d => d.ServiceType == consumerType);

        if (descriptor is null)
        {
            throw new InvalidOperationException(
                "Event consumer must be registered in the service collection."
            );
        }

        return base.AddConsumer(consumerType);
    }
}
