// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing.UnitTests;

public sealed class HashedConsumerTypesProviderTests
{
    [Fact]
    public void GetConsumerTypes_ShouldReturnCollectionOfConsumers()
    {
        HashedConsumerTypesProvider testEvent = new(
            new Dictionary<Type, IEnumerable<Type>>
            {
                { typeof(PrimarySampleConsumer), [typeof(PrimaryTestEvent)] },
                { typeof(SecondarySampleConsumer), [typeof(PrimaryTestEvent)] },
                { typeof(TertiarySampleConsumer), [typeof(SecondaryTestEvent)] },
            }
        );

        IEnumerable<Type> consumers = testEvent.GetConsumerTypes<PrimaryTestEvent>();

        _ = consumers
            .Should()
            .HaveCount(2)
            .And.Contain(typeof(PrimarySampleConsumer))
            .And.Contain(typeof(SecondarySampleConsumer));
    }

    private interface ITestEvent;

    private sealed record PrimaryTestEvent : ITestEvent;

    private sealed record SecondaryTestEvent : ITestEvent;

    private sealed record PrimarySampleConsumer : IConsumer<PrimaryTestEvent>
    {
        public ValueTask ConsumeAsync(PrimaryTestEvent payload, CancellationToken cancellationToken) =>
            ValueTask.CompletedTask;
    }

    private sealed record SecondarySampleConsumer : IConsumer<PrimaryTestEvent>
    {
        public ValueTask ConsumeAsync(PrimaryTestEvent payload, CancellationToken cancellationToken) =>
            ValueTask.CompletedTask;
    }

    private sealed record TertiarySampleConsumer : IConsumer<SecondaryTestEvent>
    {
        public ValueTask ConsumeAsync(SecondaryTestEvent payload, CancellationToken cancellationToken) =>
            ValueTask.CompletedTask;
    }
}
