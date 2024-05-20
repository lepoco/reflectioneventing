// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing;

/// <summary>
/// Provides extension methods for the <see cref="IConsumerTypesProvider"/> class.
/// </summary>
public static class ConsumerTypesProviderExtensions
{
    /// <summary>
    /// Gets the consumers for the specified event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event to get the consumers for.</typeparam>
    /// <returns>A collection of consumer types that can handle the specified event type.</returns>
    public static IEnumerable<Type> GetConsumerTypes<TEvent>(this IConsumerTypesProvider provider)
    {
        return provider.GetConsumerTypes(typeof(TEvent));
    }
}
