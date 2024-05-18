// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using Ninject;

namespace ReflectionEventing.Ninject;

/// <summary>
/// Provides event consumers for Ninject.
/// </summary>
public class NinjectConsumerProvider(IKernel kernel) : IConsumerProvider
{
    /// <inheritdoc />
    public IEnumerable<object> GetConsumerTypes(Type consumerType)
    {
        if (consumerType is null)
        {
            throw new ArgumentNullException(nameof(consumerType));
        }

        return kernel.GetAll(consumerType);
    }
}
