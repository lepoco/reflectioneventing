// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and CommandQuerySegregation Contributors.
// All Rights Reserved.

using System.Threading;
using System.Threading.Tasks;

namespace CommandQuerySegregation;

/// <summary>
/// Defines a contract for a command bus in a Command Query Responsibility Segregation (CQRS) pattern.
/// </summary>
/// <remarks>
/// A command bus is responsible for dispatching commands to their appropriate handlers.
/// </remarks>
public interface ICommandBus
{
    /// <summary>
    /// Executes the given command asynchronously.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command to execute.</typeparam>
    /// <typeparam name="TResult">The type of the result that the command returns.</typeparam>
    /// <param name="command">The command to execute.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ExecuteAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken)
        where TCommand : class;
}
