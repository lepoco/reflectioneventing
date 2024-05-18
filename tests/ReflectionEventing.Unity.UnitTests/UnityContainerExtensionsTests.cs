// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using System;
using Unity;
using Unity.Lifetime;

namespace ReflectionEventing.Unity.UnitTests;

public sealed class UnityContainerExtensionsTests
{
    [Fact]
    public void AddEventBus_RegistersServicesAndAddsConsumer()
    {
        UnityContainer container = new UnityContainer();

        _ = container.RegisterType<TestConsumer>(new HierarchicalLifetimeManager());
        _ = container.AddEventBus(builder =>
        {
            _ = builder.AddConsumer<TestConsumer>();
        });

        IConsumerTypesProvider? consumerTypesProvider = container.Resolve<IConsumerTypesProvider>();
        _ = consumerTypesProvider.Should().NotBeNull();
        _ = consumerTypesProvider.Should().BeOfType<HashedConsumerTypesProvider>();

        IConsumerProvider? consumerProvider = container.Resolve<IConsumerProvider>();
        _ = consumerProvider.Should().NotBeNull();
        _ = consumerProvider.Should().BeOfType<UnityConsumerProvider>();

        IEventBus? eventBus = container.Resolve<IEventBus>();
        _ = eventBus.Should().NotBeNull();
        _ = eventBus.Should().BeOfType<EventBus>();

        IEnumerable<Type> consumers = consumerTypesProvider!.GetConsumerTypes<TestEvent>();
        _ = consumers.First().Should().Be(typeof(TestConsumer));
    }

    public class TestConsumer : IConsumer<TestEvent>
    {
        public Task ConsumeAsync(TestEvent payload, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }

    public sealed record TestEvent;
}
