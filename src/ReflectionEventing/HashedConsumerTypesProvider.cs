// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing;

/// <summary>
/// Provides a mechanism for retrieving types of event consumers based on a specific event type.
/// </summary>
/// <remarks>
/// This class uses a dictionary of consumers where the key is the consumer type and the value is a collection of event types that the consumer can handle.
/// </remarks>
public class HashedConsumerTypesProvider(IDictionary<Type, IEnumerable<Type>> consumers)
    : IConsumerTypesProvider
{
    /// <inheritdoc />
    public IEnumerable<Type> GetConsumerTypes(Type eventType)
    {
        foreach (KeyValuePair<Type, IEnumerable<Type>> consumer in consumers)
        {
            if (consumer.Value.Contains(eventType))
            {
                yield return consumer.Key;

                continue;
            }

            // Fallback reflection
            if (!consumer.Value.Any(x => ExtendsEvent(x, eventType)))
            {
                continue;
            }

            _ = ((HashSet<Type>)consumer.Value)?.Add(eventType);

            yield return consumer.Key;
        }
    }

    private static bool ExtendsEvent(Type consumerType, Type eventType)
    {
        return consumerType.IsAssignableFrom(eventType);
    }
}
