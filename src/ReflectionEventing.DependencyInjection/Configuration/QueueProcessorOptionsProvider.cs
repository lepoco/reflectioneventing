// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing.DependencyInjection.Configuration;

/// <summary>
/// Provides configuration options for the queue processor.
/// </summary>
public sealed class QueueProcessorOptionsProvider(
    EventBusBuilderOptions options,
    object? serviceKey = null
)
{
    /// <summary>
    /// Gets the configuration options for the queue processor.
    /// </summary>
    public EventBusBuilderOptions Value
    {
        get => options;
    }

    /// <summary>
    /// Gets the service key associated with the queue processor options.
    /// </summary>
    public object? ServiceKey
    {
        get => serviceKey;
    }
}
