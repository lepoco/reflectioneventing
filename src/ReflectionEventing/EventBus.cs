// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing;

/// <summary>
/// Represents a class that provides event publishing capabilities.
/// </summary>
/// <remarks>
/// This class uses a service provider to get required services and a consumer provider to get consumers for a specific event type.
/// </remarks>
public class EventBus(
    IConsumerProvider consumerProviders,
    IConsumerTypesProvider consumerTypesProvider
) : IEventBus
{
    /// <inheritdoc />
    public async Task PublishAsync<TEvent>(TEvent eventItem, CancellationToken cancellationToken)
        where TEvent : class
    {
        if (eventItem is null)
        {
            throw new ArgumentNullException(nameof(eventItem));
        }

        Type eventType = typeof(TEvent);
        List<Task> tasks = [];
        IEnumerable<Type> consumerTypes = consumerTypesProvider.GetConsumerTypes(eventType);

        foreach (Type consumerType in consumerTypes)
        {
            foreach (object consumer in consumerProviders.GetConsumers(consumerType))
            {
                tasks.Add(((IConsumer<TEvent>)consumer).ConsumeAsync(eventItem, cancellationToken));
            }
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
