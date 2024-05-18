// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using Castle.Windsor;

namespace ReflectionEventing.Castle.Windsor;

/// <summary>
/// Represents a provider for retrieving event consumers from Castle Windsor's IoC container.
/// </summary>
public class WindsorConsumerProvider(IWindsorContainer container) : IConsumerProvider
{
    /// <inheritdoc />
    public IEnumerable<object> GetConsumerTypes(Type consumerType)
    {
        if (consumerType is null)
        {
            throw new ArgumentNullException(nameof(consumerType));
        }

        return container.ResolveAll(consumerType) as object[] ?? [];
    }
}
