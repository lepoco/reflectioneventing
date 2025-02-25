// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing;

/// <summary>
/// Specifies the mode in which events in the queue are processed.
/// </summary>
public enum ProcessingMode
{
    /// <summary>
    /// Events are processed one at a time in the order they are received.
    /// </summary>
    Sequential = 0,

    /// <summary>
    /// Events are processed concurrently, allowing multiple events to be handled at the same time.
    /// </summary>
    Parallel = 1,
}
