// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using FluentAssertions;

namespace ReflectionEventing.UnitTests;

public sealed class EventBusBuilderTests
{
    private readonly EventBusBuilder _eventBusBuilder = new EventBusBuilder();

    [Fact]
    public void AddConsumer_ShouldAddConsumerToDictionary()
    {
        Type consumerType = typeof(MySampleConsumer);

        _eventBusBuilder.AddConsumer(consumerType);

        IDictionary<Type, IEnumerable<Type>> consumers = _eventBusBuilder.GetConsumers();
        _ = consumers.Should().ContainKey(consumerType);
    }

    [Fact]
    public void AddConsumer_ShouldAddEventTypeToConsumerInDictionary()
    {
        Type consumerType = typeof(MySampleConsumer);

        _eventBusBuilder.AddConsumer(consumerType);

        IDictionary<Type, IEnumerable<Type>> consumers = _eventBusBuilder.GetConsumers();
        _ = consumers[consumerType].Should().Contain(typeof(TestEvent));
    }

    public sealed record TestEvent;

    public sealed record MySampleConsumer : IConsumer<TestEvent>
    {
        public Task ConsumeAsync(TestEvent payload, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}