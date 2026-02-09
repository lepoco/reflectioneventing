// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using Ninject;

namespace ReflectionEventing.Ninject.UnitTests;

public sealed class EventBusModuleTests
{
    [Fact]
    public void Load_RegistersServicesAndAddsConsumer()
    {
        IKernel kernel = new StandardKernel();

        _ = kernel.Bind<TestConsumer>().ToSelf().InTransientScope();

        kernel.Load(
            new EventBusModule(builder =>
            {
                _ = builder.AddConsumer<TestConsumer>();
            })
        );

        IConsumerTypesProvider? consumerTypesProvider = kernel.Get<IConsumerTypesProvider>();
        _ = consumerTypesProvider.Should().NotBeNull();
        _ = consumerTypesProvider.Should().BeOfType<HashedConsumerTypesProvider>();

        IConsumerProvider? consumerProvider = kernel.Get<IConsumerProvider>();
        _ = consumerProvider.Should().NotBeNull();
        _ = consumerProvider.Should().BeOfType<NinjectConsumerProvider>();

        IEventBus? eventBus = kernel.Get<IEventBus>();
        _ = eventBus.Should().NotBeNull();
        _ = eventBus.Should().BeOfType<EventBus>();

        IEnumerable<Type> consumers = consumerTypesProvider!.GetConsumerTypes<TestEvent>();
        _ = consumers.First().Should().Be(typeof(TestConsumer));
    }

    public class TestConsumer : IConsumer<TestEvent>
    {
        public ValueTask ConsumeAsync(TestEvent payload, CancellationToken cancellationToken) =>
            ValueTask.CompletedTask;
    }

    public sealed record TestEvent;
}
