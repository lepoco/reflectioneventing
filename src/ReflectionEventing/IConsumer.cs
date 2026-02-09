// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing;

/// <summary>
/// Defines a contract for a consumer that can handle a specific event type.
/// </summary>
/// <typeparam name="TEvent">The type of the event that the consumer can handle.</typeparam>
/// <remarks>
/// Implementations of this interface are expected to provide a mechanism to consume a specific event type asynchronously.
/// </remarks>
public interface IConsumer<in TEvent>
{
    /// <summary>
    /// Consumes the specified event asynchronously.
    /// </summary>
    /// <param name="payload">The event to consume.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A value task that represents the asynchronous operation.</returns>
    ValueTask ConsumeAsync(TEvent payload, CancellationToken cancellationToken);
}
