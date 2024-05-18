// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using Microsoft.Extensions.DependencyInjection;

namespace ReflectionEventing.DependencyInjection.UnitTests;

public sealed class DependencyInjectionEventBusBuilderTests
{
    [Fact]
    public void AddConsumer_ShouldThrowExceptionWhenConsumerNotRegistered()
    {
        IServiceCollection services = new ServiceCollection();
        DependencyInjectionEventBusBuilder eventBusBuilder = new DependencyInjectionEventBusBuilder(
            services
        );

        Action act = () => eventBusBuilder.AddConsumer(typeof(TestConsumer));

        _ = act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Event consumer must be registered in the service collection.");
    }

    [Fact]
    public void AddConsumer_ShouldAddConsumerToDictionaryWhenRegistered()
    {
        IServiceCollection services = new ServiceCollection();
        _ = services.AddSingleton<TestConsumer>();
        DependencyInjectionEventBusBuilder eventBusBuilder = new DependencyInjectionEventBusBuilder(
            services
        );

        eventBusBuilder.AddConsumer(typeof(TestConsumer));

        IDictionary<Type, IEnumerable<Type>> consumers = eventBusBuilder.GetConsumers();
        _ = consumers.Should().ContainKey(typeof(TestConsumer));
    }

    public record TestEvent;

    public class TestConsumer : IConsumer<TestEvent>
    {
        public Task ConsumeAsync(TestEvent payload, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}
