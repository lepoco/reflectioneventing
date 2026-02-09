// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using ReflectionEventing.Queues;

namespace ReflectionEventing.UnitTests;

public sealed class EventBusTests
{
    private readonly IConsumerProvider _consumerProvider;
    private readonly IConsumerTypesProvider _consumerTypesProvider;
    private readonly IEventsQueue _eventsQueue;

    public EventBusTests()
    {
        _consumerProvider = Substitute.For<IConsumerProvider>();
        _consumerTypesProvider = Substitute.For<IConsumerTypesProvider>();
        _eventsQueue = Substitute.For<IEventsQueue>();
    }

    [Fact]
    public async Task SendAsync_ShouldCallConsumeAsyncOnAllConsumers()
    {
        EventBusBuilderOptions options = new();
        EventBus eventBus = new(options, _consumerProvider, _consumerTypesProvider, _eventsQueue);

        TestEvent testEvent = new();
        Type consumerType = typeof(IConsumer<TestEvent>);
        IConsumer<TestEvent> consumer = Substitute.For<IConsumer<TestEvent>>();

        _ = _consumerTypesProvider.GetConsumerTypes<TestEvent>().Returns([consumerType]);
        _ = _consumerProvider.GetConsumers(consumerType).Returns([consumer]);

        await eventBus.SendAsync(testEvent, CancellationToken.None);

        await consumer.Received().ConsumeAsync(testEvent, Arg.Any<CancellationToken>());
    }

    [Fact]
#pragma warning disable CS0618 // Type or member is obsolete
    public async Task Send_ShouldCallSendAsync()
    {
        EventBusBuilderOptions options = new();
        EventBus eventBus = new(options, _consumerProvider, _consumerTypesProvider, _eventsQueue);

        TestEvent testEvent = new();
        Type consumerType = typeof(IConsumer<TestEvent>);
        IConsumer<TestEvent> consumer = Substitute.For<IConsumer<TestEvent>>();

        _ = _consumerTypesProvider.GetConsumerTypes<TestEvent>().Returns([consumerType]);
        _ = _consumerProvider.GetConsumers(consumerType).Returns([consumer]);

        eventBus.Send(testEvent);

        await consumer.Received().ConsumeAsync(testEvent, Arg.Any<CancellationToken>());
    }
#pragma warning restore CS0618 // Type or member is obsolete

    [Fact]
    public async Task SendAsync_WithParallelMode_ShouldExecuteConsumersConcurrently()
    {
        // Arrange
        EventBusBuilderOptions options = new() { ConsumerExecutionMode = ProcessingMode.Parallel };
        EventBus eventBus = new(options, _consumerProvider, _consumerTypesProvider, _eventsQueue);

        TestEvent testEvent = new();
        Type consumerType = typeof(IConsumer<TestEvent>);

        IConsumer<TestEvent> consumer1 = Substitute.For<IConsumer<TestEvent>>();
        IConsumer<TestEvent> consumer2 = Substitute.For<IConsumer<TestEvent>>();
        IConsumer<TestEvent> consumer3 = Substitute.For<IConsumer<TestEvent>>();

        _ = _consumerTypesProvider.GetConsumerTypes<TestEvent>().Returns([consumerType]);
        _ = _consumerProvider
            .GetConsumers(consumerType)
            .Returns([consumer1, consumer2, consumer3]);

        // Act
        await eventBus.SendAsync(testEvent, CancellationToken.None);

        // Assert
        await consumer1.Received(1).ConsumeAsync(testEvent, Arg.Any<CancellationToken>());
        await consumer2.Received(1).ConsumeAsync(testEvent, Arg.Any<CancellationToken>());
        await consumer3.Received(1).ConsumeAsync(testEvent, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendAsync_WithSequentialMode_ShouldExecuteConsumersSequentially()
    {
        // Arrange
        EventBusBuilderOptions options =
            new() { ConsumerExecutionMode = ProcessingMode.Sequential };
        EventBus eventBus = new(options, _consumerProvider, _consumerTypesProvider, _eventsQueue);

        TestEvent testEvent = new();
        Type consumerType = typeof(IConsumer<TestEvent>);

        List<int> executionOrder = [];

        IConsumer<TestEvent> consumer1 = Substitute.For<IConsumer<TestEvent>>();
        _ = consumer1
            .ConsumeAsync(testEvent, Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                executionOrder.Add(1);
                return ValueTask.CompletedTask;
            });

        IConsumer<TestEvent> consumer2 = Substitute.For<IConsumer<TestEvent>>();
        _ = consumer2
            .ConsumeAsync(testEvent, Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                executionOrder.Add(2);
                return ValueTask.CompletedTask;
            });

        IConsumer<TestEvent> consumer3 = Substitute.For<IConsumer<TestEvent>>();
        _ = consumer3
            .ConsumeAsync(testEvent, Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                executionOrder.Add(3);
                return ValueTask.CompletedTask;
            });

        _ = _consumerTypesProvider.GetConsumerTypes<TestEvent>().Returns([consumerType]);
        _ = _consumerProvider
            .GetConsumers(consumerType)
            .Returns([consumer1, consumer2, consumer3]);

        // Act
        await eventBus.SendAsync(testEvent, CancellationToken.None);

        // Assert
        await consumer1.Received(1).ConsumeAsync(testEvent, Arg.Any<CancellationToken>());
        await consumer2.Received(1).ConsumeAsync(testEvent, Arg.Any<CancellationToken>());
        await consumer3.Received(1).ConsumeAsync(testEvent, Arg.Any<CancellationToken>());

        Assert.Equal([1, 2, 3], executionOrder);
    }

    [Fact]
    public void EventBusBuilderOptions_ShouldHaveParallelAsDefaultConsumerExecutionMode()
    {
        // Arrange & Act
        EventBusBuilderOptions options = new();

        // Assert
        Assert.Equal(ProcessingMode.Parallel, options.ConsumerExecutionMode);
    }

    public sealed record TestEvent;
}
