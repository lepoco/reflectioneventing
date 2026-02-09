// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.

using System.Windows.Threading;

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
    /// Invokes the specified action on the UI thread asynchronously.
    /// If already on the UI thread, executes synchronously.
    /// </summary>
    /// <param name="action">The action to execute on the UI thread.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <param name="priority">The priority at which to invoke the action.</param>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="action"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no WPF Dispatcher is available.</exception>
    protected static ValueTask DispatchAsync(
        Action action,
        CancellationToken cancellationToken = default,
        DispatcherPriority priority = DispatcherPriority.Normal
    )
    {
        ArgumentNullException.ThrowIfNull(action);

        if (Application.Current?.Dispatcher is not { } dispatcher)
        {
            throw new InvalidOperationException(
                "No WPF Dispatcher available. Ensure Application.Current is initialized."
            );
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled(cancellationToken);
        }

        // Fast path: already on UI thread
        if (dispatcher.CheckAccess())
        {
            action();

            return ValueTask.CompletedTask;
        }

        // Slow path: dispatch to UI thread
        return new ValueTask(dispatcher.InvokeAsync(action, priority, cancellationToken).Task);
    }
}
