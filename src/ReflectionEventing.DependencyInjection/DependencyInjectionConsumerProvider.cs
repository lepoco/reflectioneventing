// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing.DependencyInjection;

/// <summary>
/// Represents a provider for retrieving event consumers from .NET Core's built-in dependency injection container.
/// </summary>
public class DependencyInjectionConsumerProvider(IServiceProvider serviceProvider)
    : IConsumerProvider
{
    /// <inheritdoc />
    public IEnumerable<object?> GetConsumers(Type consumerType)
    {
        if (consumerType is null)
        {
            throw new ArgumentNullException(nameof(consumerType));
        }

        return serviceProvider.GetServices(consumerType);
    }
}
