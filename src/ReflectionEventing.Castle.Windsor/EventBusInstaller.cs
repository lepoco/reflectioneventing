// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace ReflectionEventing.Castle.Windsor;

/// <summary>
/// Represents a Castle Windsor installer for configuring the event bus and its related services.
/// </summary>
public class EventBusInstaller(Action<WindsorEventBusBuilder> configure) : IWindsorInstaller
{
    /// <summary>
    /// Adds the event bus and its related services to the specified Windsor container.
    /// </summary>
    /// <param name="container">The <see cref="IWindsorContainer"/> to add the event bus to.</param>
    /// <param name="store">The <see cref="IConfigurationStore"/> for the container.</param>
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
        WindsorEventBusBuilder builder = new(container);

        if (!container.Kernel.HasComponent(typeof(IWindsorContainer)))
        {
            _ = container.Register(Component.For<IWindsorContainer>().Instance(container));
        }

        configure(builder);

        _ = container.Register(
            Component
                .For<IConsumerTypesProvider>()
                .Instance(builder.BuildTypesProvider())
                .LifestyleScoped(),
            Component
                .For<IConsumerProvider>()
                .ImplementedBy<WindsorConsumerProvider>()
                .LifestyleScoped(),
            Component.For<IEventBus>().ImplementedBy<EventBus>().LifestyleTransient()
        );
    }
}
