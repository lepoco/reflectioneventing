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
    /// Appends an event that failed processing to the error queue.
    /// </summary>
    /// <param name="event">The event that failed processing.</param>
    /// <param name="exception">The exception that occurred during processing.</param>
    void EnqueueError(object @event, Exception exception);

    /// <summary>
    /// Gets a reader for the event queue.
    /// </summary>
    /// <returns>A <see cref="ChannelReader{T}"/> that can be used to read events from the queue.</returns>
    ChannelReader<object> GetReader();

    /// <summary>
    /// Gets the events that failed processing from the error queue.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}"/> of events that failed processing.</returns>
    IEnumerable<FailedEvent> GetErrorQueue();
}
