// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing;

/// <summary>
/// Represents configuration options for the <see cref="EventBusBuilder"/> class.
/// These options control the behavior of the event bus built by the <see cref="EventBusBuilder"/>.
/// </summary>
public class EventBusBuilderOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the event bus should use event polymorphism.
    /// If set to <see langword="true"/>, the event bus will deliver events to consumers that handle the event type or any of its base types.
    /// If set to <see langword="false"/>, the event bus will only deliver events to consumers that handle the exact event type.
    /// The default value is <see langword="false"/>.
    /// </summary>
    public bool UseEventPolymorphism { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the event bus should use a background events queue.
    /// If set to <see langword="true"/>, the event bus will use a background queue to process events.
    /// If set to <see langword="false"/>, the event bus will process events immediately without using a background queue.
    /// The default value is <see langword="true"/>.
    /// </summary>
    public bool UseEventsQueue { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the event bus should use an error queue.
    /// If set to <see langword="true"/>, the event bus will use an error queue to handle events that fail processing.
    /// If set to <see langword="false"/>, the event bus will not use an error queue.
    /// The default value is <see langword="false"/>.
    /// </summary>
    public bool UseErrorQueue { get; set; } = false;

    /// <summary>
    /// Gets or sets the rate at which the event queue is processed.
    /// The default value is 20ms.
    /// </summary>
    /// <remarks>
    /// Adjust this value to control how frequently the event queue is processed.
    /// </remarks>
    public TimeSpan QueueTickRate { get; set; } = TimeSpan.FromMilliseconds(20); // NOTE: There are 10,000 ticks in a millisecond.

    /// <summary>
    /// Gets or sets the rate at which the error queue is processed when default queue consumption fails.
    /// The default value is 20ms.
    /// </summary>
    /// <remarks>
    /// Adjust this value to control how frequently the error queue is processed.
    /// </remarks>
    public TimeSpan ErrorTickRate { get; set; } = TimeSpan.FromMilliseconds(20); // NOTE: There are 10,000 ticks in a millisecond.
}
