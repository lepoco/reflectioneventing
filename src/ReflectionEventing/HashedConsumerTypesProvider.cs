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
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (KeyValuePair<Type, IEnumerable<Type>> consumer in consumers)
        {
            foreach (Type consumedEventType in consumer.Value)
            {
                if (consumedEventType == eventType)
                {
                    yield return consumer.Key;

                    break;
                }
            }
        }
    }
}
