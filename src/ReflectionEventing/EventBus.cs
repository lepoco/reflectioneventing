// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using ReflectionEventing.Queues;

namespace ReflectionEventing;

/// <summary>
/// Represents a class that provides event publishing capabilities.
/// </summary>
/// <remarks>
/// This class uses a service provider to get required services and a consumer provider to get consumers for a specific event type.
/// </remarks>
public class EventBus(
    IConsumerProvider consumerProviders,
    IConsumerTypesProvider consumerTypesProvider,
    IEventsQueue queue
) : IEventBus
{
    private static readonly ActivitySource ActivitySource = new("ReflectionEventing.EventBus");

    private static readonly Meter Meter = new("ReflectionEventing.EventBus");

    private static readonly Counter<long> SentCounter = Meter.CreateCounter<long>("bus.sent");

    private static readonly Counter<long> PublishedCounter = Meter.CreateCounter<long>(
        "bus.published"
    );

    /// <inheritdoc />
    public async Task SendAsync<TEvent>(
        TEvent eventItem,
        CancellationToken cancellationToken = default
    )
        where TEvent : class
    {
        using Activity? activity = ActivitySource.StartActivity(ActivityKind.Producer);

        activity?.AddTag("co.lepo.reflection.eventing.message", typeof(TEvent).Name);

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

        SentCounter.Add(1, new KeyValuePair<string, object?>("message_type", eventType.Name));
    }

    /// <inheritdoc />
    public async Task PublishAsync<TEvent>(
        TEvent eventItem,
        CancellationToken cancellationToken = default
    )
        where TEvent : class
    {
        using Activity? activity = ActivitySource.StartActivity(ActivityKind.Producer);

        activity?.AddTag("co.lepo.reflection.eventing.message", typeof(TEvent).Name);

        await queue.EnqueueAsync(eventItem, cancellationToken);

        PublishedCounter.Add(
            1,
            new KeyValuePair<string, object?>("message_type", typeof(TEvent).Name)
        );
    }
}
