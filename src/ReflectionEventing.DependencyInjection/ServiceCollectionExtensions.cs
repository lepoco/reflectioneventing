// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing.DependencyInjection;

/// <summary>
/// Provides extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the event bus and its related services to the specified services collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the event bus to.</param>
    /// <param name="configure">A delegate that configures the <see cref="EventBusBuilder"/>.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    /// <remarks>
    /// This method adds a singleton service of type <see cref="IConsumerTypesProvider"/> that uses a <see cref="HashedConsumerTypesProvider"/> with the consumers from the event bus builder.
    /// It also adds a scoped service of type <see cref="IEventBus"/> that uses the <see cref="EventBus"/> class.
    /// </remarks>
    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        Action<EventBusBuilder> configure
    )
    {
        DependencyInjectionEventBusBuilder builder = new(services);

        configure(builder);

        _ = services.AddSingleton(builder.BuildTypesProvider());
        _ = services.AddScoped<IConsumerProvider, DependencyInjectionConsumerProvider>();
        _ = services.AddScoped<IEventBus, EventBus>();

        return services;
    }
}
