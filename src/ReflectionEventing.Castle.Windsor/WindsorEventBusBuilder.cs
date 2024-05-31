// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using System.Diagnostics.CodeAnalysis;
using Castle.Windsor;

namespace ReflectionEventing.Castle.Windsor;

/// <summary>
/// Represents a builder for configuring the event bus with Castle Windsor's IoC container.
/// </summary>
public class WindsorEventBusBuilder(IWindsorContainer container) : EventBusBuilder
{
    /// <inheritdoc />
    public override EventBusBuilder AddConsumer(
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif
        Type consumerType
    )
    {
        if (!container.Kernel.HasComponent(consumerType))
        {
            throw new InvalidOperationException(
                "Event consumer must be registered in the container."
            );
        }

        return base.AddConsumer(consumerType);
    }
}
