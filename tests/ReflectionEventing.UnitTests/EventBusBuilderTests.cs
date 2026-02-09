// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using AwesomeAssertions.Collections;

namespace ReflectionEventing.UnitTests;

public sealed class EventBusBuilderTests
{
    [Fact]
    public void AddConsumer_ShouldAddConsumerToDictionary()
    {
        Type consumerType = typeof(MySampleConsumer);

        EventBusBuilder eventBusBuilder = new();
        eventBusBuilder.AddConsumer(consumerType);

        IConsumerTypesProvider typesProvider = eventBusBuilder.BuildTypesProvider();
        _ = typesProvider.GetConsumerTypes(typeof(ITestEvent)).Should().Contain(consumerType);
        _ = typesProvider.GetConsumerTypes(typeof(IBaseEvent)).Should().Contain(consumerType);
    }

    [Fact]
    public void AddConsumer_ShouldNotReturnConsumersTypes_WithoutPolymorphismEnabled()
    {
        Type consumerType = typeof(MySampleConsumer);

        EventBusBuilder eventBusBuilder = new();
        eventBusBuilder.AddConsumer(consumerType);

        IConsumerTypesProvider typesProvider = eventBusBuilder.BuildTypesProvider();
        GenericCollectionAssertions<Type>? consumers = typesProvider
            .GetConsumerTypes(typeof(TestEvent))
            .Should();
        consumers.HaveCount(0);
    }

    [Fact]
    public void AddConsumer_ShouldReturnConsumersTypes_WhenPolymorphismEnabled()
    {
        Type consumerType = typeof(MySampleConsumer);

        EventBusBuilder eventBusBuilder = new();
        eventBusBuilder.Options.UseEventPolymorphism = true;
        eventBusBuilder.AddConsumer(consumerType);

        IConsumerTypesProvider typesProvider = eventBusBuilder.BuildTypesProvider();

        GenericCollectionAssertions<Type>? consumers = typesProvider
            .GetConsumerTypes(typeof(TestEvent))
            .Should();
        consumers.HaveCount(1).And.Contain(consumerType);
    }

    [Fact]
    public void AddConsumer_ShouldReturnMultipleConsumersTypes_WhenPolymorphismEnabled()
    {
        Type primaryConsumerType = typeof(MySampleConsumer);
        Type secondaryConsumerType = typeof(MySecondarySampleConsumer);

        EventBusBuilder eventBusBuilder = new();
        eventBusBuilder.Options.UseEventPolymorphism = true;
        eventBusBuilder.AddConsumer(primaryConsumerType);
        eventBusBuilder.AddConsumer(secondaryConsumerType);

        IConsumerTypesProvider typesProvider = eventBusBuilder.BuildTypesProvider();

        GenericCollectionAssertions<Type>? consumers = typesProvider
            .GetConsumerTypes(typeof(IBaseEvent))
            .Should();
        consumers.HaveCount(2).And.Contain(primaryConsumerType).And.Contain(secondaryConsumerType);
    }

    public interface IBaseEvent;

    public interface ITestEvent;

    public sealed record TestEvent : ITestEvent, IBaseEvent;

    public sealed record SecondaryEvent : IBaseEvent;

    public sealed record MySampleConsumer : IConsumer<ITestEvent>, IConsumer<IBaseEvent>
    {
        public ValueTask ConsumeAsync(ITestEvent payload, CancellationToken cancellationToken)
        {
            return ValueTask.CompletedTask;
        }

        /// <inheritdoc />
        public ValueTask ConsumeAsync(IBaseEvent payload, CancellationToken cancellationToken)
        {
            return ValueTask.CompletedTask;
        }
    }

    public sealed record MySecondarySampleConsumer : IConsumer<IBaseEvent>
    {
        /// <inheritdoc />
        public ValueTask ConsumeAsync(IBaseEvent payload, CancellationToken cancellationToken)
        {
            return ValueTask.CompletedTask;
        }
    }
}
