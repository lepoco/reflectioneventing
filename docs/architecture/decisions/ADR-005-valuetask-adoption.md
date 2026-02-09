# ADR-005: ValueTask Adoption for Performance Optimization

## Status

**Accepted**

## Date

2026-02-09

## Context

The current async API uses `Task<T>` and `Task` return types throughout the library. While `Task` provides excellent async/await support, it has allocation overhead:

- Each `Task` instance allocates ~80 bytes on the heap
- Hot path operations (event sending) may complete synchronously
- Multiple allocations per event when no async work is needed

### Performance Characteristics

**When synchronous completion occurs:**
- Empty consumer list: Allocates Task for no work
- Single sync consumer: Consumer completes immediately
- Multiple sync consumers: Allocates Task for coordination

**ValueTask Benefits:**
- Zero allocation when completing synchronously
- Same API surface as Task
- Backward compatible at call sites (await works the same)

### When ValueTask Should Be Used (Microsoft Guidance)

✅ Method frequently completes synchronously  
✅ Called in hot paths with high frequency  
✅ Allocation overhead is measurable  
✅ Performance is a key requirement

### ValueTask Caveats

⚠️ Can only be awaited **once**  
⚠️ Cannot use with `Task.WhenAll` directly (need `.AsTask()`)  
⚠️ Slightly larger state machines for async methods  
⚠️ More complex to reason about lifetime

## Decision

Replace `Task` with `ValueTask` in all async APIs to reduce allocations in hot paths:

### API Changes

```csharp
// Before
public interface IEventBus
{
    Task SendAsync<TEvent>(TEvent eventItem, CancellationToken cancellationToken = default);
    Task PublishAsync<TEvent>(TEvent eventItem, CancellationToken cancellationToken = default);
}

public interface IConsumer<in TEvent>
{
    Task ConsumeAsync(TEvent payload, CancellationToken cancellationToken);
}

public interface IEventsQueue
{
    Task EnqueueAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default);
}

// After
public interface IEventBus
{
    ValueTask SendAsync<TEvent>(TEvent eventItem, CancellationToken cancellationToken = default);
    ValueTask PublishAsync<TEvent>(TEvent eventItem, CancellationToken cancellationToken = default);
}

public interface IConsumer<in TEvent>
{
    ValueTask ConsumeAsync(TEvent payload, CancellationToken cancellationToken);
}

public interface IEventsQueue
{
    ValueTask EnqueueAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default);
}
```

### Parallel Execution Strategy

For `SendAsync` with multiple consumers, use a custom helper to maintain parallel execution without excessive allocations:

```csharp
// EventBus.SendAsync implementation
List<ValueTask> tasks = [];
foreach (Type consumerType in consumerTypes)
{
    foreach (object? consumer in consumerProviders.GetConsumers(consumerType))
    {
        tasks.Add(((IConsumer<TEvent>)consumer).ConsumeAsync(eventItem, cancellationToken));
    }
}

// Use helper method for parallel execution
await WhenAll(tasks).ConfigureAwait(false);

// Helper method
private static async ValueTask WhenAll(List<ValueTask> tasks)
{
    if (tasks.Count == 0)
        return;
        
    if (tasks.Count == 1)
    {
        await tasks[0].ConfigureAwait(false);
        return;
    }
    
    // Only allocate Task[] when truly needed
    await Task.WhenAll(tasks.Select(t => t.AsTask())).ConfigureAwait(false);
}
```

### Version and Breaking Change

This is a **BREAKING CHANGE**:
- All consumer implementations must change signatures
- Version bump: 4.x → **5.0.0** (major version)
- Migration guide required

## Consequences

### Positive

✅ **Reduced allocations** - No Task allocation when completing synchronously  
✅ **Better performance** - Hot path operations (send/publish) allocate less  
✅ **Modern .NET pattern** - Aligns with .NET 5+ best practices  
✅ **Scalability** - Less GC pressure under high load  
✅ **API surface unchanged** - await still works the same way

### Negative

❌ **Breaking change** - All consumers must update signatures  
❌ **Migration required** - Users must update to v5.0.0  
❌ **Complexity** - Slightly more complex to implement correctly  
❌ **Tooling** - Some older analyzers may not understand ValueTask  
❌ **State machine size** - Slightly larger IL for async methods

### Neutral

↔️ **Compatibility** - `netstandard2.0` supported via `System.Threading.Tasks.Extensions`  
↔️ **Learning curve** - Developers must understand ValueTask semantics  
↔️ **Testing** - No impact on test structure

## Implementation Notes

### Target Frameworks

All supported frameworks have ValueTask:
- `net8.0` - Native support
- `netstandard2.0` - Via `System.Threading.Tasks.Extensions` (already referenced)

### Migration Path

1. Update all interfaces to return `ValueTask`
2. Update all implementations
3. Update all consumers in tests and demos
4. Update documentation and examples
5. Bump version to 5.0.0
6. Add migration guide

### Performance Testing

Benchmark scenarios:
- Empty consumer list (sync completion)
- Single sync consumer
- Single async consumer
- Multiple mixed consumers

Expected improvements primarily in sync completion paths.

## Alternatives Considered

### Alternative 1: Keep Task
**Rejected** - Does not address allocation overhead in hot paths

### Alternative 2: Provide both Task and ValueTask
**Rejected** - Doubles API surface, confusing for users

### Alternative 3: Sequential execution in SendAsync
**Revisited in v5.1.0** - Originally rejected to maintain backward compatibility. Later implemented as a configurable option via `EventBusBuilderOptions.ConsumerExecutionMode` to provide flexibility for use cases where execution order matters or where parallel execution causes issues.

## References

- [Understanding the Whys, Whats, and Whens of ValueTask (Microsoft)](https://devblogs.microsoft.com/dotnet/understanding-the-whys-whats-and-whens-of-valuetask/)
- [Task vs ValueTask Performance](https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Threading/Tasks/ValueTask.cs)
- ADR-003: Async-First Design for All Operations

## Related

- Supersedes: None
- Relates to: ADR-003 (Async-First Design)
- Breaking change: Requires major version bump (4.x → 5.0.0)
