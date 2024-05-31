// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing;

/// <summary>
/// Provides event publishing capabilities.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publishes the specified event asynchronously.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to publish.</typeparam>
    /// <param name="eventItem">The event to publish.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// This method gets the consumers for the specified event type from the consumer provider and then uses the service provider to get the required service for each consumer.
    /// Each consumer is then used to consume the event asynchronously.
    /// </remarks>
    Task PublishAsync<TEvent>(TEvent eventItem, CancellationToken cancellationToken)
        where TEvent : class;
}
