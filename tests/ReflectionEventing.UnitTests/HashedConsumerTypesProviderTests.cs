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
        HashedConsumerTypesProvider testEvent =
            new(
                new Dictionary<Type, IEnumerable<Type>>
                {
                    { typeof(PrimarySampleConsumer), [typeof(TestEvent)] },
                    { typeof(SecondarySampleConsumer), [typeof(TestEvent)] },
                }
            );

        IEnumerable<Type> consumers = testEvent.GetConsumerTypes<TestEvent>();

        _ = consumers.Should().HaveCount(2);
        _ = consumers.Should().Contain(typeof(PrimarySampleConsumer));
    }

    public sealed record TestEvent;

    public sealed record PrimarySampleConsumer : IConsumer<TestEvent>
    {
        public Task ConsumeAsync(TestEvent payload, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }

    public sealed record SecondarySampleConsumer : IConsumer<TestEvent>
    {
        public Task ConsumeAsync(TestEvent payload, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}
