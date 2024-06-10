// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using Autofac;

namespace ReflectionEventing.Autofac;

/// <summary>
/// Provides event consumers for Autofac.
/// </summary>
public class AutofacConsumerProvider(ILifetimeScope lifetimeScope) : IConsumerProvider
{
    /// <inheritdoc />
    public IEnumerable<object> GetConsumers(Type consumerType)
    {
        if (consumerType is null)
        {
            throw new ArgumentNullException(nameof(consumerType));
        }

        return new List<object> { lifetimeScope.Resolve(consumerType) };
    }
}
