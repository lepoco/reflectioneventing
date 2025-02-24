// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing;

/// <summary>
/// Defines a provider for retrieving types of event consumers.
/// </summary>
/// <remarks>
/// An implementation of this interface should be able to provide all types that are consumers of a specific event type.
/// The consumers are not necessarily instances, but rather the types that can be used to create instances of consumers.
/// </remarks>
public interface IConsumerProvider
{
    /// <summary>
    /// Gets the consumers objects for the specified event type.
    /// </summary>
    /// <param name="consumerType">The type of the event that the consumers handle.</param>
    /// <returns>An enumerable of <see cref="object"/>'s that are consumers of the specified event type.</returns>
    /// <example>
    /// <code>
    /// Type consumerType = typeof(MyEvent);
    /// IEnumerable&lt;object&gt; consumerTypes = consumerProvider.GetConsumers(consumerType);
    /// </code>
    /// </example>
    IEnumerable<object?> GetConsumers(Type consumerType);
}
