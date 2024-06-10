// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and CommandQuerySegregation Contributors.
// All Rights Reserved.

using System.Threading;
using System.Threading.Tasks;

namespace CommandQuerySegregation;

/// <summary>
/// Defines a contract for a query bus in a Command Query Responsibility Segregation (CQRS) pattern.
/// </summary>
/// <remarks>
/// A query bus is responsible for dispatching queries to their appropriate handlers.
/// </remarks>
public interface IQueryBus
{
    /// <summary>
    /// Executes the given query asynchronously.
    /// </summary>
    /// <typeparam name="TQuery">The type of the query to execute.</typeparam>
    /// <typeparam name="TResult">The type of the result that the query returns.</typeparam>
    /// <param name="query">The query to execute.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ExecuteAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken)
        where TQuery : class;
}
