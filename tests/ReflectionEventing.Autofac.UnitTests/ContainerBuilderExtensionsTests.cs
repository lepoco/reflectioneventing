// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using Autofac;

namespace ReflectionEventing.Autofac.UnitTests;

public sealed class ContainerBuilderExtensionsTests
{
    [Fact]
    public void AddEventBus_RegistersServicesAndAddsConsumer()
    {
        ContainerBuilder builder = new ContainerBuilder();

        _ = builder.RegisterType<TestConsumer>().AsSelf();
        _ = builder.AddEventBus(eventBusBuilder =>
        {
            _ = eventBusBuilder.AddConsumer<TestConsumer>();
        });

        IContainer container = builder.Build();

        IConsumerTypesProvider consumerTypesProvider = container.Resolve<IConsumerTypesProvider>();
        _ = consumerTypesProvider.Should().NotBeNull();
        _ = consumerTypesProvider.Should().BeOfType<HashedConsumerTypesProvider>();

        IConsumerProvider consumerProvider = container.Resolve<IConsumerProvider>();
        _ = consumerProvider.Should().NotBeNull();
        _ = consumerProvider.Should().BeOfType<AutofacConsumerProvider>();

        IEventBus eventBus = container.Resolve<IEventBus>();
        _ = eventBus.Should().NotBeNull();
        _ = eventBus.Should().BeOfType<EventBus>();

        IEnumerable<Type> consumers = consumerTypesProvider.GetConsumerTypes<TestEvent>();
        _ = consumers.Should().ContainSingle().Which.Should().Be(typeof(TestConsumer));
    }

    public class TestConsumer : IConsumer<TestEvent>
    {
        public Task ConsumeAsync(TestEvent payload, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }

    public sealed record TestEvent;
}
