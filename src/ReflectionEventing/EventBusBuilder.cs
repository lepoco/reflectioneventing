// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing;

/// <summary>
/// Represents a class that builds an event bus with a specific set of classConsumers.
/// </summary>
/// <remarks>
/// This class uses a dictionary of classConsumers where the key is the consumer type and the value is a collection of event types that the consumer can handle.
/// </remarks>
public class EventBusBuilder
{
    private readonly IDictionary<Type, IEnumerable<Type>> classConsumers =
        new Dictionary<Type, IEnumerable<Type>>();

    /// <summary>
    /// Gets or sets a value indicating whether the event bus should use event polymorphism.
    /// If set to true, the event bus will deliver events to classConsumers that handle the event type or any of its base types.
    /// If set to false, the event bus will only deliver events to classConsumers that handle the exact event type.
    /// The default value is false.
    /// </summary>
    public EventBusBuilderOptions Options { get; } = new();

    /// <summary>
    /// Builds and returns an instance of <see cref="IConsumerTypesProvider"/> based on the current configuration.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="IConsumerTypesProvider"/>. If <see cref="EventBusBuilderOptions.UseEventPolymorphism"/> is set to true,
    /// it returns an instance of <see cref="HashedPolymorphicConsumerTypesProvider"/>, otherwise it returns an instance of <see cref="HashedConsumerTypesProvider"/>.
    /// </returns>
    public IConsumerTypesProvider BuildTypesProvider()
    {
        return Options.UseEventPolymorphism
            ? new HashedPolymorphicConsumerTypesProvider(classConsumers)
            : new HashedConsumerTypesProvider(classConsumers);
    }

    /// <summary>
    /// Adds a consumer to the builder.
    /// </summary>
    /// <param name="consumerType">The type of the consumer to add.</param>
    /// <remarks>
    /// This method checks if the consumer is registered in the service collection and if it is not transient.
    /// It then gets the interfaces of the consumer that are generic and have a generic type definition of <see cref="IConsumer{TEvent}"/>.
    /// For each of these interfaces, it gets the generic argument and adds it to the classConsumers dictionary.
    /// </remarks>
    public virtual EventBusBuilder AddConsumer(
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif
        Type consumerType
    )
    {
        if (consumerType is null)
        {
            throw new ArgumentNullException(nameof(consumerType));
        }

        IEnumerable<Type> consumerInterfaces = consumerType
            .GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>));

        foreach (Type consumerInterface in consumerInterfaces)
        {
            Type consumedEventType = consumerInterface.GetGenericArguments()[0];

            if (!classConsumers.ContainsKey(consumerType))
            {
                classConsumers[consumerType] = new HashSet<Type>();
            }

            _ = ((HashSet<Type>)classConsumers[consumerType]).Add(consumedEventType);
        }

        return this;
    }
}
