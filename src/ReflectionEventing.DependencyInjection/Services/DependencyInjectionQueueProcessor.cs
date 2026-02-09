// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using ReflectionEventing.DependencyInjection.Configuration;
using ReflectionEventing.Queues;

namespace ReflectionEventing.DependencyInjection.Services;

public class DependencyInjectionQueueProcessor(
    IEventsQueue queue,
    IServiceScopeFactory scopeFactory,
    QueueProcessorOptionsProvider options,
    ILogger<DependencyInjectionQueueProcessor> logger
) : BackgroundService
{
    private static readonly ActivitySource ActivitySource = new(
        "ReflectionEventing.QueueProcessor"
    );

    private static readonly Meter Meter = new("ReflectionEventing.QueueProcessor");

    private static readonly Counter<long> EventsProcessed = Meter.CreateCounter<long>(
        "bus.processed"
    );

    private static readonly Counter<long> EventsFailed = Meter.CreateCounter<long>("bus.failed");

    private readonly TimeSpan tickRate = options.Value.QueueTickRate;

    private readonly TimeSpan errorTickRate = options.Value.ErrorTickRate;

    private readonly SemaphoreSlim semaphore = new(options.Value.ConcurrentTaskLimit);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await BackgroundProcessing(cancellationToken);
    }

    protected virtual async Task BackgroundProcessing(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await ProcessQueueAsync(cancellationToken);

                await Task.Delay(tickRate, cancellationToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error occurred during queue processing");

                await Task.Delay(errorTickRate, cancellationToken);
            }
        }
    }

    protected virtual async Task ProcessQueueAsync(CancellationToken cancellationToken)
    {
        using Activity? activity = ActivitySource.StartActivity(ActivityKind.Consumer);

#if NET8_0_OR_GREATER
        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
#else
        using IServiceScope? scope = scopeFactory.CreateScope();
#endif

#if NET8_0_OR_GREATER
        IConsumerProvider consumerProvider = options.ServiceKey is null
            ? scope.ServiceProvider.GetRequiredService<IConsumerProvider>()
            : scope.ServiceProvider.GetRequiredKeyedService<IConsumerProvider>(options.ServiceKey);
        IConsumerTypesProvider consumerTypesProvider = options.ServiceKey is null
            ? scope.ServiceProvider.GetRequiredService<IConsumerTypesProvider>()
            : scope.ServiceProvider.GetRequiredKeyedService<IConsumerTypesProvider>(
                options.ServiceKey
            );
#else
        IConsumerProvider consumerProvider =
            scope.ServiceProvider.GetRequiredService<IConsumerProvider>();
        IConsumerTypesProvider consumerTypesProvider =
            scope.ServiceProvider.GetRequiredService<IConsumerTypesProvider>();
#endif

        await foreach (object @event in queue.ReadEventsAsync(cancellationToken))
        {
            await ProcessEventAsync(
                @event,
                consumerProvider,
                consumerTypesProvider,
                activity,
                cancellationToken
            );
        }
    }

    private async Task ProcessEventAsync(
        object @event,
        IConsumerProvider consumerProvider,
        IConsumerTypesProvider consumerTypesProvider,
        Activity? activity,
        CancellationToken cancellationToken
    )
    {
        Type eventType = @event.GetType();

        IEnumerable<Type> consumerTypes = consumerTypesProvider.GetConsumerTypes(eventType);

        foreach (Type consumerType in consumerTypes)
        {
            foreach (object? consumer in consumerProvider.GetConsumers(consumerType))
            {
                if (consumer is null)
                {
                    return;
                }

                if (options.Value.QueueMode == ProcessingMode.Sequential)
                {
                    await ExecuteConsumerAsync(
                        @event,
                        consumerType,
                        eventType,
                        consumer,
                        activity,
                        cancellationToken
                    );
                }
                else if (options.Value.QueueMode == ProcessingMode.Parallel)
                {
                    await semaphore.WaitAsync(cancellationToken);

                    _ = Task.Run(
                        async () =>
                        {
                            try
                            {
                                await ExecuteConsumerAsync(
                                    @event,
                                    consumerType,
                                    eventType,
                                    consumer,
                                    activity,
                                    cancellationToken
                                );
                            }
                            catch (Exception e)
                            {
                                logger.LogError(e, "Error occurred during consumer execution");
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        },
                        cancellationToken
                    );
                }
                else
                {
                    throw new InvalidOperationException(
                        "Invalid queue processing mode. Must be either Sequential or Parallel."
                    );
                }
            }
        }

        EventsProcessed.Add(1, new KeyValuePair<string, object?>("message_type", eventType.Name));
    }

    private async Task ExecuteConsumerAsync(
        object @event,
        Type consumerType,
        Type eventType,
        object consumer,
        Activity? activity,
        CancellationToken cancellationToken
    )
    {
        MethodInfo? consumeMethod = consumerType.GetMethod(
            "ConsumeAsync",
            [@event.GetType(), typeof(CancellationToken)]
        );

        if (consumeMethod != null)
        {
            try
            {
                object? result = consumeMethod.Invoke(consumer, [@event, cancellationToken]);

                // Handle ValueTask return type
                if (result is ValueTask valueTask)
                {
                    await valueTask.ConfigureAwait(false);
                }
                else if (result is Task task)
                {
                    await task.ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                //activity?.AddException(e);
                activity?.SetStatus(ActivityStatusCode.Error);

                logger.LogError(
                    new EventId(75001, "ReflectionEventingQueueProcessingFailed"),
                    e,
                    "Error processing event of type {EventName}",
                    @event.GetType().Name
                );

                if (options.Value.UseErrorQueue)
                {
                    queue.EnqueueError(
                        new FailedEvent
                        {
                            Data = @event,
                            Exception = e,
                            Timestamp = DateTimeOffset.UtcNow,
                            FailedConsumer = consumerType,
                        }
                    );
                }

                EventsFailed.Add(
                    1,
                    new KeyValuePair<string, object?>("message_type", eventType.Name)
                );
            }
        }
        else
        {
            logger.LogError(
                new EventId(75002, "ReflectionEventingConsumerMissing"),
                "ConsumeAsync method not found on consumer {ConsumerType} for event type {EventName}",
                consumerType.Name,
                @event.GetType().Name
            );
        }
    }
}
