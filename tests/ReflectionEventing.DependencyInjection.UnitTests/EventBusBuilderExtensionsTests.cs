// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing.DependencyInjection.UnitTests;

public sealed class EventBusBuilderExtensionsTests
{
    [Fact]
    public void AddConsumer_ShouldRegisterConsumerWhenLifetimeProvided()
    {
        IServiceCollection services = new ServiceCollection();
        DependencyInjectionEventBusBuilder eventBusBuilder = new(services);

        eventBusBuilder.AddConsumer<TestConsumer>(ServiceLifetime.Transient);

        IConsumerTypesProvider consumerTypesProvider = eventBusBuilder.BuildTypesProvider();
        _ = consumerTypesProvider
            .GetConsumerTypes(typeof(TestEvent))
            .Should()
            .Contain(typeof(TestConsumer));

        services
            .First(d => d.ServiceType == typeof(TestConsumer))
            .Lifetime.Should()
            .Be(ServiceLifetime.Transient);
    }

    [Fact]
    public void AddScopedConsumer_ShouldThrowExceptionWhenConsumerIsRegisteredWithDifferentScope()
    {
        IServiceCollection services = new ServiceCollection();
        _ = services.AddSingleton<TestConsumer>();
        DependencyInjectionEventBusBuilder eventBusBuilder = new(services);

        Action act = () => eventBusBuilder.AddScopedConsumer(typeof(TestConsumer));

        _ = act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage(
                "Event consumer must be registered with the same lifetime as the one provided."
            );
    }

    [Fact]
    public void AddScopedConsumer_ShouldRegisterTransientConsumer()
    {
        IServiceCollection services = new ServiceCollection();
        DependencyInjectionEventBusBuilder eventBusBuilder = new(services);

        eventBusBuilder.AddTransientConsumer<TestConsumer>();

        IConsumerTypesProvider consumerTypesProvider = eventBusBuilder.BuildTypesProvider();
        _ = consumerTypesProvider
            .GetConsumerTypes(typeof(TestEvent))
            .Should()
            .Contain(typeof(TestConsumer));

        services
            .First(d => d.ServiceType == typeof(TestConsumer))
            .Lifetime.Should()
            .Be(ServiceLifetime.Transient);
    }

    [Fact]
    public void AddScopedConsumer_ShouldRegisterScopedConsumer()
    {
        IServiceCollection services = new ServiceCollection();
        DependencyInjectionEventBusBuilder eventBusBuilder = new(services);

        eventBusBuilder.AddScopedConsumer<TestConsumer>();

        IConsumerTypesProvider consumerTypesProvider = eventBusBuilder.BuildTypesProvider();
        _ = consumerTypesProvider
            .GetConsumerTypes(typeof(TestEvent))
            .Should()
            .Contain(typeof(TestConsumer));

        services
            .First(d => d.ServiceType == typeof(TestConsumer))
            .Lifetime.Should()
            .Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddScopedConsumer_ShouldRegisterSingletonConsumer()
    {
        IServiceCollection services = new ServiceCollection();
        DependencyInjectionEventBusBuilder eventBusBuilder = new(services);

        eventBusBuilder.AddSingletonConsumer<TestConsumer>();

        IConsumerTypesProvider consumerTypesProvider = eventBusBuilder.BuildTypesProvider();
        _ = consumerTypesProvider
            .GetConsumerTypes(typeof(TestEvent))
            .Should()
            .Contain(typeof(TestConsumer));

        services
            .First(d => d.ServiceType == typeof(TestConsumer))
            .Lifetime.Should()
            .Be(ServiceLifetime.Singleton);
    }

    public interface ITestEvent;

    public record TestEvent : ITestEvent;

    public class TestConsumer : IConsumer<TestEvent>
    {
        public Task ConsumeAsync(TestEvent payload, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}
