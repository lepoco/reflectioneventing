// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using ReflectionEventing.Queues;

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
    public async Task PublishAsync_ThrowsExceptionWhenQueueIsDisabled()
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
                        builder.Options.UseEventsQueue = false;

                        builder.AddSingletonConsumer<TestConsumer>();
                    });
                }
            )
            .Build();

        await host.StartAsync();

        IEventBus bus = host.Services.GetRequiredService<IEventBus>();

        Func<Task> action = async () => await bus.PublishAsync(new OtherEvent());

        await action
            .Should()
            .ThrowAsync<QueueException>()
            .WithMessage("The background queue processor is disabled.");
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

    [Fact]
    public async Task PublishAsync_SwallowsConsumerException()
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

                        builder.AddSingletonConsumer<FailingConsumer>();
                    });
                }
            )
            .Build();

        await host.StartAsync();

        IEventBus bus = host.Services.GetRequiredService<IEventBus>();

        await bus.PublishAsync(new TestEvent());

        await Task.Delay(TimeSpan.FromSeconds(1));

        IEventsQueue queue = host.Services.GetRequiredService<IEventsQueue>();
        IEnumerable<FailedEvent> errors = queue.GetErrors();

        errors.Should().HaveCount(0);

        await host.StopAsync();
    }

    [Fact]
    public async Task PublishAsync_MovesToErrorQueue()
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
                        builder.Options.UseErrorQueue = true;

                        builder.AddSingletonConsumer<FailingConsumer>();
                    });
                }
            )
            .Build();

        await host.StartAsync();

        IEventBus bus = host.Services.GetRequiredService<IEventBus>();

        await bus.PublishAsync(new TestEvent());

        await Task.Delay(TimeSpan.FromSeconds(1));

        IEventsQueue queue = host.Services.GetRequiredService<IEventsQueue>();
        FailedEvent[] errors = queue.GetErrors().ToArray();

        errors.Should().HaveCount(1);
        errors.First().Data.Should().BeOfType<TestEvent>();

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

        public ValueTask ConsumeAsync(TestEvent payload, CancellationToken cancellationToken)
        {
            TestEventConsumed = true;
            return ValueTask.CompletedTask;
        }

        public ValueTask ConsumeAsync(OtherEvent payload, CancellationToken cancellationToken)
        {
            OtherEventConsumed = true;
            return ValueTask.CompletedTask;
        }

        public ValueTask ConsumeAsync(AsyncQueuedEvent payload, CancellationToken cancellationToken)
        {
            AsyncQueuedEventConsumed = true;
            return ValueTask.CompletedTask;
        }
    }

    public class FailingConsumer : IConsumer<TestEvent>
    {
        public ValueTask ConsumeAsync(TestEvent payload, CancellationToken cancellationToken)
        {
            throw new Exception("Consumer failed.");
        }
    }
}
