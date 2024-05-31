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
    /// If set to true, the event bus will deliver events to consumers that handle the event type or any of its base types.
    /// If set to false, the event bus will only deliver events to consumers that handle the exact event type.
    /// The default value is false.
    /// </summary>
    public bool UseEventPolymorphism { get; set; } = false;
}
