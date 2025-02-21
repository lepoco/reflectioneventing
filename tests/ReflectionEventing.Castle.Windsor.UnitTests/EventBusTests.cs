// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace ReflectionEventing.Castle.Windsor.UnitTests;

public sealed class EventBusTests : IDisposable
{
    private readonly IWindsorContainer _container;

    public EventBusTests()
    {
        _container = new WindsorContainer();
        _ = _container.Register(Component.For<TestConsumer>().LifestyleScoped());

        EventBusInstaller installer = new(builder =>
        {
            _ = builder.AddConsumer<TestConsumer>();
        });

        installer.Install(_container, null!);
    }

    [Fact]
    public async Task SendAsync_ShouldCallConsumeAsyncOnAllConsumers()
    {
        using IDisposable scope = _container.BeginScope();
        IEventBus eventBus = _container.Resolve<IEventBus>();

        await eventBus.SendAsync(new TestEvent(), CancellationToken.None);

        _ = _container.Resolve<TestConsumer>().ReceivedEvents.Should().Be(1);
    }

    public void Dispose()
    {
        _container.Dispose();
    }

    public class TestConsumer : IConsumer<TestEvent>
    {
        public int ReceivedEvents { get; private set; } = 0;

        public Task ConsumeAsync(TestEvent payload, CancellationToken cancellationToken)
        {
            ReceivedEvents++;

            return Task.CompletedTask;
        }
    }

    public sealed record TestEvent;
}
