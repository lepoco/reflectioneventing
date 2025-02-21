// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing.DependencyInjection.Configuration;

/// <summary>
/// Provides configuration options for the event bus, specifically the tick rate.
/// </summary>
public sealed class QueueProcessorOptionsProvider(TimeSpan tickRate, TimeSpan errorTickRate)
{
    /// <summary>
    /// Gets the tick rate for the event bus queue.
    /// </summary>
    public TimeSpan TickRate
    {
        get => tickRate;
    }

    /// <summary>
    /// Gets the error tick rate for the event bus queue.
    /// </summary>
    public TimeSpan ErrorTickRate
    {
        get => errorTickRate;
    }
}
