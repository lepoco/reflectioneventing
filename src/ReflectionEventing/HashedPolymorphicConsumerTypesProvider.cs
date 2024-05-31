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
public class HashedPolymorphicConsumerTypesProvider(IDictionary<Type, IEnumerable<Type>> consumers)
    : IConsumerTypesProvider
{
    /// <inheritdoc />
    public IEnumerable<Type> GetConsumerTypes(Type eventType)
    {
        foreach (KeyValuePair<Type, IEnumerable<Type>> consumer in consumers)
        {
            bool consumerHasRelatedType = false;

            foreach (Type consumedEventType in consumer.Value)
            {
                if (consumedEventType == eventType)
                {
                    yield return consumer.Key;

                    continue;
                }

                if (AreTypesRelated(consumedEventType, eventType))
                {
                    consumerHasRelatedType = true;
                }
            }

            if (!consumerHasRelatedType)
            {
                continue;
            }

            if (consumer.Value is HashSet<Type> consumersHashSet)
            {
                _ = consumersHashSet.Add(eventType);
            }
            else if (consumer.Value is ICollection<Type> consumersCollection)
            {
                consumersCollection.Add(eventType);
            }

            yield return consumer.Key;
        }
    }

    private static bool AreTypesRelated(Type type1, Type type2)
    {
        return type1.IsAssignableFrom(type2) || type2.IsAssignableFrom(type1);
    }
}
