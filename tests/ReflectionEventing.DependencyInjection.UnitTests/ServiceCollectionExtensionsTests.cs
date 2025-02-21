// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using Microsoft.Extensions.DependencyInjection;

namespace ReflectionEventing.DependencyInjection.UnitTests;

public sealed class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddEventBus_RegistersServicesAndAddsConsumer()
    {
        ServiceCollection services = new();

        _ = services.AddScoped<TestConsumer>();
        _ = services.AddEventBus(builder =>
        {
            _ = builder.AddConsumer<TestConsumer>();
        });

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        IConsumerTypesProvider? consumerTypesProvider =
            serviceProvider.GetService<IConsumerTypesProvider>();
        _ = consumerTypesProvider.Should().NotBeNull();
        _ = consumerTypesProvider.Should().BeOfType<HashedConsumerTypesProvider>();

        IConsumerProvider? consumerProvider = serviceProvider.GetService<IConsumerProvider>();
        _ = consumerProvider.Should().NotBeNull();
        _ = consumerProvider.Should().BeOfType<DependencyInjectionConsumerProvider>();

        IEventBus? eventBus = serviceProvider.GetService<IEventBus>();
        _ = eventBus.Should().NotBeNull();
        _ = eventBus.Should().BeOfType<EventBus>();

        IEnumerable<Type> consumers = consumerTypesProvider!.GetConsumerTypes<TestEvent>();
        _ = consumers.First().Should().Be(typeof(TestConsumer));
    }

    public class TestConsumer : IConsumer<TestEvent>
    {
        public Task ConsumeAsync(TestEvent payload, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }

    public sealed record TestEvent;
}
