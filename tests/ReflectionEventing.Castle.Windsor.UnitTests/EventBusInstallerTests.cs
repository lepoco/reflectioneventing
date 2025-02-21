// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace ReflectionEventing.Castle.Windsor.UnitTests;

public sealed class EventBusInstallerTests
{
    [Fact]
    public void Install_RegistersServicesAndAddsConsumer()
    {
        IWindsorContainer container = new WindsorContainer();

        _ = container.Register(Component.For<TestConsumer>().LifestyleScoped());

        EventBusInstaller installer = new(builder =>
        {
            _ = builder.AddConsumer<TestConsumer>();
        });

        installer.Install(container, null!);

        using IDisposable scope = container.BeginScope();

        IConsumerTypesProvider consumerTypesProvider = container.Resolve<IConsumerTypesProvider>();
        _ = consumerTypesProvider.Should().NotBeNull();
        _ = consumerTypesProvider.Should().BeOfType<HashedConsumerTypesProvider>();

        IConsumerProvider consumerProvider = container.Resolve<IConsumerProvider>();
        _ = consumerProvider.Should().NotBeNull();
        _ = consumerProvider.Should().BeOfType<WindsorConsumerProvider>();

        IEventBus eventBus = container.Resolve<IEventBus>();
        _ = eventBus.Should().NotBeNull();
        _ = eventBus.Should().BeOfType<EventBus>();

        IEnumerable<Type> consumers = consumerTypesProvider.GetConsumerTypes<TestEvent>();

        _ = consumers.First().Should().Be(typeof(TestConsumer));
    }

    public class TestConsumer : IConsumer<TestEvent>
    {
        public Task ConsumeAsync(TestEvent payload, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }

    public sealed record TestEvent;
}
