// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using ReflectionEventing.Demo.Wpf.Events;

namespace ReflectionEventing.Demo.Wpf.Services;

internal sealed class BackgroundTickService(IEventBus eventBus) : IHostedService
{
    private const int TickRateInMilliseconds = 100;

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Run(() => TickInBackground(cancellationToken), cancellationToken)
            .ConfigureAwait(false);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task TickInBackground(CancellationToken cancellationToken)
    {
        Random random = new();

        while (true)
        {
            await eventBus.PublishAsync(
                new BackgroundTicked(random.Next(10, 1001)),
                cancellationToken
            );

            await Task.Delay(TickRateInMilliseconds, cancellationToken);
        }
    }
}
