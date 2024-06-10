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
        DependencyInjectionEventBusBuilder eventBusBuilder = new(services);

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
        DependencyInjectionEventBusBuilder eventBusBuilder = new(services);

        eventBusBuilder.AddConsumer(typeof(TestConsumer));

        IConsumerTypesProvider consumerTypesProvider = eventBusBuilder.BuildTypesProvider();
        _ = consumerTypesProvider
            .GetConsumerTypes(typeof(TestEvent))
            .Should()
            .Contain(typeof(TestConsumer));
    }

    public interface ITestEvent;

    public record TestEvent : ITestEvent;

    public class TestConsumer : IConsumer<TestEvent>
    {
        public Task ConsumeAsync(TestEvent payload, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}
