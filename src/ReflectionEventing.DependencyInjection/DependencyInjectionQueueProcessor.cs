// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using ReflectionEventing.DependencyInjection.Configuration;
using ReflectionEventing.Queues;

namespace ReflectionEventing.DependencyInjection;

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

    private readonly TimeSpan tickRate = options.TickRate;

    private readonly TimeSpan errorTickRate = options.ErrorTickRate;

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
                await ProcessQueueAsync(queue.GetReader(), cancellationToken);

                await Task.Delay(tickRate, cancellationToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error occurred during queue processing");

                await Task.Delay(errorTickRate, cancellationToken);
            }
        }
    }

    protected virtual async Task ProcessQueueAsync(
        ChannelReader<object> reader,
        CancellationToken cancellationToken
    )
    {
        if (reader.Count < 1)
        {
            return;
        }

        using Activity? activity = ActivitySource.StartActivity(ActivityKind.Consumer);

#if NET8_0_OR_GREATER
        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
#else
        using IServiceScope? scope = scopeFactory.CreateScope();
#endif

        IConsumerProvider? consumerProvider =
            scope.ServiceProvider.GetRequiredService<IConsumerProvider>();
        IConsumerTypesProvider? consumerTypesProvider =
            scope.ServiceProvider.GetRequiredService<IConsumerTypesProvider>();

        await foreach (object @event in reader.ReadAllAsync(cancellationToken))
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

        try
        {
            List<Task> batch = [];

            foreach (Type consumerType in consumerTypes)
            {
                foreach (object consumer in consumerProvider.GetConsumers(consumerType))
                {
                    MethodInfo? consumeMethod = consumerType.GetMethod(
                        "ConsumeAsync",
                        [@event.GetType(), typeof(CancellationToken)]
                    );

                    if (consumeMethod != null)
                    {
                        batch.Add(
                            (Task)consumeMethod.Invoke(consumer, [@event, cancellationToken])
                        );
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

            await Task.WhenAll(batch);

            EventsProcessed.Add(
                1,
                new KeyValuePair<string, object?>("message_type", eventType.Name)
            );
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

            queue.EnqueueError(@event, e);

            EventsFailed.Add(1, new KeyValuePair<string, object?>("message_type", eventType.Name));
        }
    }
}
