// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing.Queues;

/// <summary>
/// Defines a contract for an event queue that supports asynchronous operations for appending and retrieving events.
/// </summary>
public interface IEventsQueue
{
    /// <summary>
    /// Appends an event to the queue asynchronously.
    /// </summary>
    /// <param name="event">The event to append to the queue.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous append operation.</returns>
    Task EnqueueAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class;

    /// <summary>
    /// Appends a failed event to the error queue.
    /// </summary>
    /// <param name="fail">The failed event to append to the error queue.</param>
    void EnqueueError(FailedEvent fail);

    /// <summary>
    /// Reads the events from the queue asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of events from the queue.</returns>
    IAsyncEnumerable<object> ReadEventsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets the events that failed processing from the error queue.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}"/> of events that failed processing.</returns>
    IEnumerable<FailedEvent> GetErrors();
}
