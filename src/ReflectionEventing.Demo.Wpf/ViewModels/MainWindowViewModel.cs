// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using ReflectionEventing.Demo.Wpf.Events;

namespace ReflectionEventing.Demo.Wpf.ViewModels;

public partial class MainWindowViewModel
    : ObservableObject,
        IConsumer<ITickedEvent>,
        IConsumer<OtherEvent>
{
    [ObservableProperty]
    private int _currentTick = 0;

    /// <inheritdoc />
    public async Task ConsumeAsync(ITickedEvent payload, CancellationToken cancellationToken)
    {
        int tickValue = payload.Value;

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            CurrentTick = tickValue;
        });
    }

    /// <inheritdoc />
    public async Task ConsumeAsync(OtherEvent payload, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
