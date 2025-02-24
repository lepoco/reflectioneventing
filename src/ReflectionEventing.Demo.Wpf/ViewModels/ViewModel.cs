// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

namespace ReflectionEventing.Demo.Wpf.ViewModels;

/// <summary>
/// Represents an abstract ViewModel base class that provides functionality for dispatching actions on the UI thread.
/// </summary>
/// <remarks>
/// This class extends the ObservableObject class to provide property change notification.
/// </remarks>
public abstract class ViewModel : ObservableObject
{
    /// <summary>
    /// Dispatches the specified action on the UI thread.
    /// </summary>
    /// <param name="action">The action to be dispatched.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected static async Task DispatchAsync(Action action, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        if (Application.Current is null)
        {
            return;
        }

        await Application.Current.Dispatcher.InvokeAsync(action);
    }
}
