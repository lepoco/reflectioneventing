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
    public void Publish<TEvent>(TEvent eventItem)
    {
        Task.Run(() =>
            {
                using CancellationTokenSource cancellationSource = new();

                PublishAsync(eventItem, cancellationSource.Token)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            })
            .GetAwaiter()
            .GetResult();
    }

    /// <inheritdoc />
    public async Task PublishAsync<TEvent>(TEvent eventItem, CancellationToken cancellationToken)
    {
        List<Task> tasks = new();
        IEnumerable<Type> consumerTypes = consumerTypesProvider.GetConsumerTypes<TEvent>();

        foreach (Type consumerType in consumerTypes)
        {
            IEnumerable<IConsumer<TEvent>> consumerObjects = consumerProviders
                .GetConsumerTypes(consumerType)
                .OfType<IConsumer<TEvent>>();

            tasks.AddRange(
                consumerObjects.Select(x => x.ConsumeAsync(eventItem, cancellationToken))
            );
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
