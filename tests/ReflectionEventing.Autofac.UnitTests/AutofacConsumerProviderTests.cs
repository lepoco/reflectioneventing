// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using Autofac;

namespace ReflectionEventing.Autofac.UnitTests;

public sealed class AutofacConsumerProviderTests
{
    [Fact]
    public void GetConsumerTypes_ShouldThrowExceptionWhenConsumerTypeIsNull()
    {
        ILifetimeScope lifetimeScope = Substitute.For<ILifetimeScope>();
        AutofacConsumerProvider consumerProvider = new AutofacConsumerProvider(lifetimeScope);

        Action act = () => consumerProvider.GetConsumers(null!);

        _ = act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'consumerType')");
    }

    [Fact]
    public void GetConsumerTypes_ShouldReturnResolvedConsumerType()
    {
        TestConsumer testInstance = new();

        ContainerBuilder builder = new ContainerBuilder();
        _ = builder.RegisterInstance(testInstance).As<TestConsumer>().SingleInstance();
        IContainer container = builder.Build();
        ILifetimeScope scope = container.BeginLifetimeScope();

        AutofacConsumerProvider consumerProvider = new AutofacConsumerProvider(scope);

        IEnumerable<object> actualConsumers = consumerProvider.GetConsumers(typeof(TestConsumer));

        _ = actualConsumers.First().Should().Be(testInstance);
    }

    public sealed record TestEvent;

    public sealed class TestConsumer : IConsumer<TestEvent>
    {
        public ValueTask ConsumeAsync(TestEvent payload, CancellationToken cancellationToken) =>
            ValueTask.CompletedTask;
    }
}
