// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using ReflectionEventing.DependencyInjection.Configuration;
using ReflectionEventing.Queues;

namespace ReflectionEventing.DependencyInjection;

public class DependencyInjectionEventBus(
    EventBusBuilderOptions options,
    IConsumerProvider consumerProviders,
    IConsumerTypesProvider consumerTypesProvider,
    IEventsQueue queue
) : EventBus(options, consumerProviders, consumerTypesProvider, queue)
{
    /// <inheritdoc />
    public override ValueTask PublishAsync<TEvent>(
        TEvent eventItem,
        CancellationToken cancellationToken = default
    )
    {
        if (!options.UseEventsQueue)
        {
            throw new QueueException("The background queue processor is disabled.");
        }

        return base.PublishAsync(eventItem, cancellationToken);
    }
}
