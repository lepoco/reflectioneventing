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
public class EventBus(IServiceProvider serviceProvider, IConsumerProvider consumerProvider)
    : IEventBus
{
    /// <inheritdoc />
    public void Publish<TEvent>(TEvent eventItem)
    {
        using CancellationTokenSource cancellationSource = new();

        PublishAsync(eventItem, cancellationSource.Token).GetAwaiter().GetResult();
    }

    /// <inheritdoc />
    public async Task PublishAsync<TEvent>(TEvent eventItem, CancellationToken cancellationToken)
    {
        foreach (Type serviceType in consumerProvider.GetConsumers<TEvent>())
        {
            // NOTE: Here we activate the model instances that are listening for events but are not active yet. Is this a desirable result? I don't know, at the moment it fulfills basic functionalities.
            if (serviceProvider.GetRequiredService(serviceType) is IConsumer<TEvent> consumer)
            {
                await consumer.ConsumeAsync(eventItem, cancellationToken);
            }
        }
    }
}
