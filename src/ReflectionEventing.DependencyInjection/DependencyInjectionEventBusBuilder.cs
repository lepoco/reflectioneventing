// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using System.Diagnostics.CodeAnalysis;

namespace ReflectionEventing.DependencyInjection;

/// <summary>
/// Represents a builder for configuring the event bus with .NET Core's built-in dependency injection.
/// </summary>
public class DependencyInjectionEventBusBuilder(IServiceCollection services) : EventBusBuilder
{
    /// <inheritdoc />
    public override EventBusBuilder AddConsumer(
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif
        Type consumerType
    )
    {
        ServiceDescriptor? descriptor = services.FirstOrDefault(d => d.ServiceType == consumerType);

        if (descriptor is null)
        {
            throw new InvalidOperationException(
                "Event consumer must be registered in the service collection."
            );
        }

        return base.AddConsumer(consumerType);
    }
}
