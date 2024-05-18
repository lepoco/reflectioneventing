// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using Ninject.Modules;

namespace ReflectionEventing.Ninject;

/// <summary>
/// Represents a Ninject module for configuring the event bus.
/// </summary>
public class EventBusModule(Action<NinjectEventBusBuilder> configure) : NinjectModule
{
    /// <summary>
    /// Loads the module into the kernel.
    /// </summary>
    public override void Load()
    {
        NinjectEventBusBuilder builder = new(Kernel);

        configure(builder);

        _ = Bind<IConsumerTypesProvider>()
            .To<HashedConsumerTypesProvider>()
            .InSingletonScope()
            .WithConstructorArgument("consumers", builder.GetConsumers());

        _ = Bind<IConsumerProvider>().To<NinjectConsumerProvider>().InTransientScope();

        _ = Bind<IEventBus>().To<EventBus>().InTransientScope();
    }
}
