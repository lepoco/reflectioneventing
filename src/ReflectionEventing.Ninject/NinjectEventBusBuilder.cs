// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using Ninject;

namespace ReflectionEventing.Ninject;

/// <summary>
/// Represents a builder for configuring the event bus with Ninject.
/// </summary>
public class NinjectEventBusBuilder(IKernel kernel) : EventBusBuilder
{
    /// <inheritdoc />
    public override void AddConsumer(Type consumerType)
    {
        if (!kernel.GetBindings(consumerType).Any())
        {
            throw new InvalidOperationException("Event consumer must be registered in the kernel.");
        }

        base.AddConsumer(consumerType);
    }
}
