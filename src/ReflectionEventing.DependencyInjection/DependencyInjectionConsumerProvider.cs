// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing.DependencyInjection;

public class DependencyInjectionConsumerProvider(IServiceProvider serviceProvider)
    : IConsumerProvider
{
    public IEnumerable<object> GetConsumerTypes(Type consumerType)
    {
        if (consumerType is null)
        {
            throw new ArgumentNullException(nameof(consumerType));
        }

        return serviceProvider.GetServices(consumerType);
    }
}
