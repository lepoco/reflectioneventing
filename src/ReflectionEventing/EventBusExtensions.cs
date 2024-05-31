// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing;

/// <summary>
/// Provides extension methods for the <see cref="IEventBus"/>.
/// </summary>
public static class EventBusExtensions
{
    /// <summary>
    /// Publishes the specified event synchronously.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to publish.</typeparam>
    /// <param name="eventBus">The event bus to extend.</param>
    /// <param name="eventItem">The event to publish.</param>
    [Obsolete($"May cause deadlock on UI threads, use {nameof(IEventBus.PublishAsync)} instead.")]
    public static void Publish<TEvent>(this IEventBus eventBus, TEvent eventItem)
        where TEvent : class
    {
        using CancellationTokenSource cancellationSource = new();

        Task.Run(
                () =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    eventBus
                        .PublishAsync(eventItem, cancellationSource.Token)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                },
                cancellationSource.Token
            )
            .GetAwaiter()
            .GetResult();
    }
}
