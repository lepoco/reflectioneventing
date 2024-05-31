// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing.UnitTests;

public sealed class EventBusTests
{
    private readonly IConsumerProvider _consumerProvider;
    private readonly IConsumerTypesProvider _consumerTypesProvider;
    private readonly EventBus _eventBus;

    public EventBusTests()
    {
        _consumerProvider = Substitute.For<IConsumerProvider>();
        _consumerTypesProvider = Substitute.For<IConsumerTypesProvider>();
        _eventBus = new EventBus(_consumerProvider, _consumerTypesProvider);
    }

    [Fact]
    public async Task PublishAsync_ShouldCallConsumeAsyncOnAllConsumers()
    {
        TestEvent testEvent = new();
        Type consumerType = typeof(IConsumer<TestEvent>);
        IConsumer<TestEvent> consumer = Substitute.For<IConsumer<TestEvent>>();

        _ = _consumerTypesProvider.GetConsumerTypes<TestEvent>().Returns([consumerType]);
        _ = _consumerProvider.GetConsumers(consumerType).Returns([consumer]);

        await _eventBus.PublishAsync(testEvent, CancellationToken.None);

        await consumer.Received().ConsumeAsync(testEvent, Arg.Any<CancellationToken>());
    }

    [Fact]
#pragma warning disable CS0618 // Type or member is obsolete
    public async Task Publish_ShouldCallPublishAsync()
    {
        TestEvent testEvent = new();
        Type consumerType = typeof(IConsumer<TestEvent>);
        IConsumer<TestEvent> consumer = Substitute.For<IConsumer<TestEvent>>();

        _ = _consumerTypesProvider.GetConsumerTypes<TestEvent>().Returns([consumerType]);
        _ = _consumerProvider.GetConsumers(consumerType).Returns([consumer]);

        _eventBus.Publish(testEvent);

        await consumer.Received().ConsumeAsync(testEvent, Arg.Any<CancellationToken>());
    }
#pragma warning restore CS0618 // Type or member is obsolete

    public sealed record TestEvent;
}
