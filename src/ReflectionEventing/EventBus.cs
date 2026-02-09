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
    public virtual ValueTask SendAsync<TEvent>(
        TEvent eventItem,
        CancellationToken cancellationToken = default
    )
        where TEvent : class
    {
        if (eventItem is null)
        {
            throw new ArgumentNullException(nameof(eventItem));
        }

        using Activity? activity = ActivitySource.StartActivity(ActivityKind.Producer);

        _ = activity?.AddTag("co.lepo.reflection.eventing.message", typeof(TEvent).Name);

        Type eventType = typeof(TEvent);
        IEnumerable<Type> consumerTypes = consumerTypesProvider.GetConsumerTypes(eventType);

        // Collect all consumers first
        List<object> consumers = [];
        foreach (Type consumerType in consumerTypes)
        {
            foreach (object? consumer in consumerProviders.GetConsumers(consumerType))
            {
                if (consumer is not null)
                {
                    consumers.Add(consumer);
                }
            }
        }

        SentCounter.Add(1, new KeyValuePair<string, object?>("message_type", eventType.Name));

        // Execute based on consumer count - optimized paths
        if (consumers.Count == 0)
        {
            return default;
        }

        if (consumers.Count == 1)
        {
            return ((IConsumer<TEvent>)consumers[0]).ConsumeAsync(eventItem, cancellationToken);
        }

        // Multiple consumers - collect tasks and await all
        List<ValueTask> tasks = new(consumers.Count);
        foreach (object consumer in consumers)
        {
            tasks.Add(((IConsumer<TEvent>)consumer).ConsumeAsync(eventItem, cancellationToken));
        }

        return WhenAll(tasks);
    }

    /// <inheritdoc />
    public virtual ValueTask PublishAsync<TEvent>(
        TEvent eventItem,
        CancellationToken cancellationToken = default
    )
        where TEvent : class
    {
        using Activity? activity = ActivitySource.StartActivity(ActivityKind.Producer);

        _ = activity?.AddTag("co.lepo.reflection.eventing.message", typeof(TEvent).Name);

        PublishedCounter.Add(
            1,
            new KeyValuePair<string, object?>("message_type", typeof(TEvent).Name)
        );

        return queue.EnqueueAsync(eventItem, cancellationToken);
    }

    /// <summary>
    /// Waits for all ValueTasks to complete in parallel.
    /// </summary>
    /// <param name="tasks">The list of ValueTasks to wait for (must contain 2 or more tasks).</param>
    /// <returns>A ValueTask that completes when all tasks have completed.</returns>
    private static ValueTask WhenAll(List<ValueTask> tasks) =>
        new(Task.WhenAll(tasks.Select(t => t.AsTask())));
}
