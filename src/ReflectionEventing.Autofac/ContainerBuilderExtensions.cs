// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using System;
using Autofac;

namespace ReflectionEventing.Autofac;

public static class ContainerBuilderExtensions
{
    public static ContainerBuilder AddEventBus(
        this ContainerBuilder builder,
        Action<EventBusBuilder> configure
    )
    {
        AutofacEventBusBuilder autofacBuilder = new(builder);

        configure(autofacBuilder);

        _ = builder
            .RegisterType<HashedConsumerTypesProvider>()
            .As<IConsumerTypesProvider>()
            .WithParameter("consumers", autofacBuilder.GetConsumers())
            .SingleInstance();

        _ = builder
            .RegisterType<AutofacConsumerProvider>()
            .As<IConsumerProvider>()
            .InstancePerLifetimeScope();

        _ = builder.RegisterType<EventBus>().As<IEventBus>().InstancePerLifetimeScope();

        return builder;
    }
}
