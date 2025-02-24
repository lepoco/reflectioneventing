// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing.Queues;

/// <summary>
/// Represents an exception that occurs during queue operations.
/// </summary>
public class QueueException(string message) : EventBusException(message);
