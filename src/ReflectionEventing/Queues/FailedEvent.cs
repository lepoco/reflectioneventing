// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing.Queues;

/// <summary>
/// Represents an event that failed processing, including the event data, the exception that occurred, the consumer that failed, and the timestamp of the failure.
/// </summary>
public sealed record FailedEvent
{
    /// <summary>
    /// Gets the data of the event that failed processing.
    /// </summary>
    public required object Data { get; init; }

    /// <summary>
    /// Gets the exception that occurred during processing.
    /// </summary>
    public required Exception Exception { get; init; }

    /// <summary>
    /// Gets the type of the consumer that failed to process the event.
    /// </summary>
    public required Type FailedConsumer { get; init; }

    /// <summary>
    /// Gets the timestamp of when the failure occurred.
    /// </summary>
    public required DateTimeOffset Timestamp { get; init; }
}
