// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing;

/// <summary>
/// Defines a contract for a provider that can supply consumers for a specific event type.
/// </summary>
/// <remarks>
/// Implementations of this interface are expected to provide a mechanism to retrieve consumers that can handle a specific event type.
/// </remarks>
public interface IConsumerTypesProvider
{
    /// <summary>
    /// Gets the consumer types for the specified event type.
    /// </summary>
    /// <returns>A collection of consumer types that can handle the specified event type.</returns>
    IEnumerable<Type> GetConsumerTypes(Type eventType);
}
