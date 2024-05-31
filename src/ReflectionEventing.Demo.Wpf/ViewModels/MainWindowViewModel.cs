// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using ReflectionEventing.Demo.Wpf.Events;

namespace ReflectionEventing.Demo.Wpf.ViewModels;

public partial class MainWindowViewModel(ILogger<MainWindowViewModel> logger)
    : ViewModel,
        IConsumer<ITickedEvent>,
        IConsumer<OtherEvent>
{
    [ObservableProperty]
    private int _currentTick;

    /// <inheritdoc />
    public async Task ConsumeAsync(ITickedEvent payload, CancellationToken cancellationToken)
    {
        int tickValue = payload.Value;

        await DispatchAsync(
            () =>
            {
                CurrentTick = tickValue;
            },
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task ConsumeAsync(OtherEvent payload, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received {Event} event.", nameof(OtherEvent));

        await Task.CompletedTask;
    }
}
