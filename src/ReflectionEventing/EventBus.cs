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
    EventBusBuilderOptions options,
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

        // Defer List allocation: track first consumer in a local variable
        object? singleConsumer = null;
        List<object>? consumers = null;

        foreach (Type consumerType in consumerTypes)
        {
            foreach (object? consumer in consumerProviders.GetConsumers(consumerType))
            {
                if (consumer is null)
                {
                    continue;
                }

                if (singleConsumer is null)
                {
                    singleConsumer = consumer;
                }
                else
                {
                    consumers ??= new List<object> { singleConsumer };
                    consumers.Add(consumer);
                }
            }
        }

        SentCounter.Add(1, new KeyValuePair<string, object?>("message_type", eventType.Name));

        // Execute based on consumer count - optimized paths
        if (singleConsumer is null)
        {
            return default;
        }

        if (consumers is null)
        {
            return ((IConsumer<TEvent>)singleConsumer).ConsumeAsync(eventItem, cancellationToken);
        }

        // Multiple consumers - execute based on configured mode
        return options.ConsumerExecutionMode == ProcessingMode.Sequential
            ? ExecuteSequentialAsync(consumers, eventItem, cancellationToken)
            : ExecuteParallelAsync(consumers, eventItem, cancellationToken);
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
    /// Executes consumers sequentially, one at a time.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="consumers">The list of consumers to execute.</param>
    /// <param name="eventItem">The event to consume.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ValueTask that completes when all consumers have completed.</returns>
    private static async ValueTask ExecuteSequentialAsync<TEvent>(
        List<object> consumers,
        TEvent eventItem,
        CancellationToken cancellationToken
    )
        where TEvent : class
    {
        foreach (object consumer in consumers)
        {
            await ((IConsumer<TEvent>)consumer)
                .ConsumeAsync(eventItem, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Executes consumers in parallel using Task.WhenAll.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="consumers">The list of consumers to execute.</param>
    /// <param name="eventItem">The event to consume.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ValueTask that completes when all consumers have completed.</returns>
    private static ValueTask ExecuteParallelAsync<TEvent>(
        List<object> consumers,
        TEvent eventItem,
        CancellationToken cancellationToken
    )
        where TEvent : class
    {
        List<ValueTask>? asyncTasks = null;

        // First pass: execute all synchronous completions
        foreach (object consumer in consumers)
        {
            ValueTask task = ((IConsumer<TEvent>)consumer).ConsumeAsync(
                eventItem,
                cancellationToken
            );

            if (!task.IsCompletedSuccessfully)
            {
                asyncTasks ??= new List<ValueTask>(consumers.Count);
                asyncTasks.Add(task);
            }
        }

        // Only allocate Task objects for truly async operations
        if (asyncTasks is null)
        {
            return default;
        }

        Task[] tasks = new Task[asyncTasks.Count];
        for (int i = 0; i < asyncTasks.Count; i++)
        {
            tasks[i] = asyncTasks[i].AsTask();
        }

        return new ValueTask(Task.WhenAll(tasks));
    }
}
