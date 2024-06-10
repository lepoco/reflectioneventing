// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using Autofac;

namespace ReflectionEventing.Autofac;

/// <summary>
/// Provides extension methods for the <see cref="ContainerBuilder"/> class.
/// </summary>
public static class ContainerBuilderExtensions
{
    /// <summary>
    /// Adds the event bus and its related services to the specified Autofac container builder.
    /// </summary>
    /// <param name="builder">The <see cref="ContainerBuilder"/> to add the event bus to.</param>
    /// <param name="configure">A delegate that configures the <see cref="EventBusBuilder"/>.</param>
    /// <returns>The same container builder so that multiple calls can be chained.</returns>
    /// <remarks>
    /// This method adds a singleton service of type <see cref="IConsumerTypesProvider"/> that uses a <see cref="HashedConsumerTypesProvider"/> with the consumers from the event bus builder.
    /// It also adds a scoped service of type <see cref="IEventBus"/> that uses the <see cref="EventBus"/> class.
    /// </remarks>
    public static ContainerBuilder AddEventBus(
        this ContainerBuilder builder,
        Action<EventBusBuilder> configure
    )
    {
        AutofacEventBusBuilder autofacBuilder = new(builder);

        configure(autofacBuilder);

        _ = builder
            .RegisterInstance(autofacBuilder.BuildTypesProvider())
            .As<IConsumerTypesProvider>()
            .SingleInstance();

        _ = builder
            .RegisterType<AutofacConsumerProvider>()
            .As<IConsumerProvider>()
            .InstancePerLifetimeScope();

        _ = builder.RegisterType<EventBus>().As<IEventBus>().InstancePerLifetimeScope();

        return builder;
    }
}
