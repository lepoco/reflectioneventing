// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using System.Diagnostics.CodeAnalysis;

namespace ReflectionEventing;

/// <summary>
/// Represents a class that builds an event bus with a specific set of consumers.
/// This class is sealed and cannot be inherited.
/// </summary>
/// <remarks>
/// This class uses a dictionary of consumers where the key is the consumer type and the value is a collection of event types that the consumer can handle.
/// </remarks>
public sealed class EventBusBuilder(IServiceCollection services)
{
    private readonly IDictionary<Type, IEnumerable<Type>> consumers =
        new Dictionary<Type, IEnumerable<Type>>();

    /// <summary>
    /// Gets the consumers that have been added to the builder.
    /// </summary>
    /// <returns>A dictionary of consumers where the key is the consumer type and the value is a collection of event types that the consumer can handle.</returns>
    public IDictionary<Type, IEnumerable<Type>> GetConsumers()
    {
        return consumers;
    }

    /// <summary>
    /// Adds a consumer to the builder.
    /// </summary>
    /// <param name="consumerType">The type of the consumer to add.</param>
    /// <remarks>
    /// This method checks if the consumer is registered in the service collection and if it is not transient.
    /// It then gets the interfaces of the consumer that are generic and have a generic type definition of <see cref="IConsumer{TEvent}"/>.
    /// For each of these interfaces, it gets the generic argument and adds it to the consumers dictionary.
    /// </remarks>
    public void AddConsumer(
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

        if (descriptor.Lifetime == ServiceLifetime.Transient)
        {
            throw new InvalidOperationException("Transient consumers are not supported.");
        }

        IEnumerable<Type> consumerInterfaces = consumerType
            .GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>));

        foreach (Type consumerInterface in consumerInterfaces)
        {
            Type consumedEventType = consumerInterface.GetGenericArguments()[0];

            if (!consumers.ContainsKey(consumedEventType))
            {
                consumers[consumerType] = new HashSet<Type>();
            }

            _ = ((HashSet<Type>)consumers[consumerType]).Add(consumedEventType);
        }
    }
}
