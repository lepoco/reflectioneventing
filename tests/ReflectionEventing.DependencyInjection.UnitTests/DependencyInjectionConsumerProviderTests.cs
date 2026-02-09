// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing.DependencyInjection.UnitTests;

public sealed class DependencyInjectionConsumerProviderTests
{
    [Fact]
    public void GetConsumerTypes_ShouldThrowExceptionWhenConsumerTypeIsNull()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        DependencyInjectionConsumerProvider consumerProvider = new(serviceProvider);

        Action act = () => consumerProvider.GetConsumers(null!);

        _ = act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'consumerType')");
    }

    [Fact]
    public void GetConsumerTypes_ShouldReturnServicesOfConsumerType()
    {
        IServiceCollection services = new ServiceCollection();
        _ = services.AddSingleton<TestConsumer>();
        _ = services.AddSingleton<TestConsumer>();
        DependencyInjectionConsumerProvider consumerProvider = new(services.BuildServiceProvider());

        IEnumerable<object?> actualConsumers = consumerProvider.GetConsumers(typeof(TestConsumer));

        _ = actualConsumers.Should().HaveCount(2);
    }

    public record TestEvent;

    public class TestConsumer : IConsumer<TestEvent>
    {
        public ValueTask ConsumeAsync(TestEvent payload, CancellationToken cancellationToken) =>
            ValueTask.CompletedTask;
    }
}
