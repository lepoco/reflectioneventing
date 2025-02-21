// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing.Queues;

public class EventsQueue : IEventsQueue
{
    private readonly Channel<object> events = Channel.CreateUnbounded<object>();

    private readonly ConcurrentQueue<FailedEvent> errorQueue = new();

    /// <inheritdoc />
    public virtual async Task EnqueueAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken = default
    )
        where TEvent : class
    {
        await events.Writer.WriteAsync(@event, cancellationToken);
    }

    /// <inheritdoc />
    public ChannelReader<object> GetReader()
    {
        return events.Reader;
    }

    /// <inheritdoc />
    public void EnqueueError(object @event, Exception exception)
    {
        errorQueue.Enqueue(new FailedEvent { Data = @event, Exception = exception });
    }

    /// <inheritdoc />
    public IEnumerable<FailedEvent> GetErrorQueue()
    {
        return errorQueue;
    }
}
