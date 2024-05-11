// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing;

/// <summary>
/// Represents a class that provides consumers for a specific event type.
/// This class is sealed and cannot be inherited.
/// </summary>
/// <remarks>
/// This class uses a dictionary of consumers where the key is the consumer type and the value is a collection of event types that the consumer can handle.
/// </remarks>
public class HashedConsumerTypesProvider(IDictionary<Type, IEnumerable<Type>> consumers)
    : IConsumerTypesProvider
{
    /// <inheritdoc />
    public IEnumerable<Type> GetConsumerTypes<TEvent>()
    {
        return consumers.Where(x => x.Value.Contains(typeof(TEvent))).Select(x => x.Key);
    }
}
