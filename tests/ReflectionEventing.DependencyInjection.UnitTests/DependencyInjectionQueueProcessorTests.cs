// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing.DependencyInjection.UnitTests;

public sealed class DependencyInjectionQueueProcessorTests
{
    [Fact]
    public async Task PublishAsync_ShouldNotSendEventsToWrongCustomers()
    {
        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices(
                (_, services) =>
                {
                    services.AddEventBus(builder =>
                    {
                        builder.Options.QueueTickRate = TimeSpan.FromTicks(10_000);
                        builder.Options.ErrorTickRate = TimeSpan.FromTicks(10_000);
                        builder.Options.UseEventPolymorphism = true;

                        builder.AddSingletonConsumer<TestConsumer>();
                    });
                }
            )
            .Build();

        await host.StartAsync();

        IEventBus bus = host.Services.GetRequiredService<IEventBus>();

        await bus.PublishAsync(new VoidEvent());

        await Task.Delay(TimeSpan.FromSeconds(1));

        TestConsumer testConsumer = host.Services.GetRequiredService<TestConsumer>();

        testConsumer.TestEventConsumed.Should().BeFalse();
        testConsumer.OtherEventConsumed.Should().BeFalse();
        testConsumer.AsyncQueuedEventConsumed.Should().BeFalse();

        await host.StopAsync();
    }

    [Fact]
    public async Task PublishAsync_ShouldSendOnlyOneEvent()
    {
        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices(
                (_, services) =>
                {
                    services.AddEventBus(builder =>
                    {
                        builder.Options.QueueTickRate = TimeSpan.FromTicks(10_000);
                        builder.Options.ErrorTickRate = TimeSpan.FromTicks(10_000);
                        builder.Options.UseEventPolymorphism = true;

                        builder.AddSingletonConsumer<TestConsumer>();
                    });
                }
            )
            .Build();

        await host.StartAsync();

        IEventBus bus = host.Services.GetRequiredService<IEventBus>();

        await bus.PublishAsync(new OtherEvent());

        await Task.Delay(TimeSpan.FromSeconds(1));

        TestConsumer testConsumer = host.Services.GetRequiredService<TestConsumer>();

        testConsumer.TestEventConsumed.Should().BeFalse();
        testConsumer.OtherEventConsumed.Should().BeTrue();
        testConsumer.AsyncQueuedEventConsumed.Should().BeFalse();

        await host.StopAsync();
    }

    [Fact]
    public async Task PublishAsync_ShouldProperlyAddEventsToQueue()
    {
        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices(
                (_, services) =>
                {
                    services.AddEventBus(builder =>
                    {
                        builder.Options.QueueTickRate = TimeSpan.FromTicks(10_000);
                        builder.Options.ErrorTickRate = TimeSpan.FromTicks(10_000);
                        builder.Options.UseEventPolymorphism = true;

                        builder.AddSingletonConsumer<TestConsumer>();
                    });
                }
            )
            .Build();

        await host.StartAsync();

        IEventBus bus = host.Services.GetRequiredService<IEventBus>();

        await bus.PublishAsync(new TestEvent());
        await bus.PublishAsync(new OtherEvent());
        await bus.PublishAsync(new AsyncQueuedEvent());

        await Task.Delay(TimeSpan.FromSeconds(1));

        TestConsumer testConsumer = host.Services.GetRequiredService<TestConsumer>();

        testConsumer.TestEventConsumed.Should().BeTrue();
        testConsumer.OtherEventConsumed.Should().BeTrue();
        testConsumer.AsyncQueuedEventConsumed.Should().BeTrue();

        await host.StopAsync();
    }

    public record TestEvent;

    public record OtherEvent;

    public record AsyncQueuedEvent;

    public record VoidEvent;

    public class TestConsumer
        : IConsumer<TestEvent>,
            IConsumer<OtherEvent>,
            IConsumer<AsyncQueuedEvent>
    {
        public bool TestEventConsumed { get; private set; }
        public bool OtherEventConsumed { get; private set; }
        public bool AsyncQueuedEventConsumed { get; private set; }

        public Task ConsumeAsync(TestEvent payload, CancellationToken cancellationToken)
        {
            TestEventConsumed = true;
            return Task.CompletedTask;
        }

        public Task ConsumeAsync(OtherEvent payload, CancellationToken cancellationToken)
        {
            OtherEventConsumed = true;
            return Task.CompletedTask;
        }

        public Task ConsumeAsync(AsyncQueuedEvent payload, CancellationToken cancellationToken)
        {
            AsyncQueuedEventConsumed = true;
            return Task.CompletedTask;
        }
    }

    public class FailingConsumer : IConsumer<TestEvent>
    {
        public Task ConsumeAsync(TestEvent payload, CancellationToken cancellationToken)
        {
            throw new Exception("Consumer failed.");
        }
    }
}
