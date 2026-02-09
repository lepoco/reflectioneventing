# Plan: Implement ValueTask for Performance Optimization

**Data:** 2026-02-09
**Status:** DRAFT

## 1. Overview

Replace `Task` with `ValueTask` in async methods throughout the codebase to reduce allocations when operations complete synchronously.

### When ValueTask is Beneficial (from Microsoft Docs)
- Method is expected to **complete synchronously frequently**
- Method is called in **hot paths** (high frequency)
- Allocation overhead of `Task` is measurable

### Caveats
- ⚠️ **BREAKING CHANGE** - All consumers must update their implementations
- ⚠️ `ValueTask` can only be awaited **once**
- ⚠️ Cannot use with `Task.WhenAll` directly (need `.AsTask()`)
- ⚠️ Larger state machines for async methods

## 2. Scope Analysis

| Question | Answer |
|----------|--------|
| Które warstwy? | Core library + all DI integrations |
| Nowy ADR potrzebny? | **Tak** - ADR-005-valuetask-adoption.md |
| Zmiana kontraktu API? | **Tak** - wszystkie interfejsy |
| Migracja bazy danych? | Nie |
| Breaking change? | **Tak - MAJOR version bump required** |

## 3. Zakres zmian

| Warstwa | Zmiany | Pliki |
|---------|--------|-------|
| Core Interfaces | Change return types from `Task` to `ValueTask` | `IEventBus.cs`, `IConsumer.cs`, `IEventsQueue.cs` |
| Core Implementation | Update implementations | `EventBus.cs`, `EventsQueue.cs` |
| DependencyInjection | Update DI-specific implementations | `DependencyInjectionEventBus.cs`, `DependencyInjectionQueueProcessor.cs` |
| Autofac | Update provider | (if has custom EventBus) |
| Castle.Windsor | Update provider | (if has custom EventBus) |
| Ninject | Update provider | (if has custom EventBus) |
| Unity | Update provider | (if has custom EventBus) |
| Demo | Update consumers | `MainWindowViewModel.cs`, `BackgroundTickService.cs` |
| Tests | Update all test consumers and assertions | All `*Tests.cs` files |
| Documentation | Update examples and ADRs | `README.md`, docs/ |

## 4. Interface Changes

### IEventBus.cs
```csharp
// Before
Task SendAsync<TEvent>(TEvent eventItem, CancellationToken cancellationToken = default);
Task PublishAsync<TEvent>(TEvent eventItem, CancellationToken cancellationToken = default);

// After
ValueTask SendAsync<TEvent>(TEvent eventItem, CancellationToken cancellationToken = default);
ValueTask PublishAsync<TEvent>(TEvent eventItem, CancellationToken cancellationToken = default);
```

### IConsumer.cs
```csharp
// Before
Task ConsumeAsync(TEvent payload, CancellationToken cancellationToken);

// After
ValueTask ConsumeAsync(TEvent payload, CancellationToken cancellationToken);
```

### IEventsQueue.cs
```csharp
// Before
Task EnqueueAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default);

// After
ValueTask EnqueueAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default);
```

## 5. Implementation Notes

### EventBus.cs Changes
```csharp
// Problem: Task.WhenAll doesn't work with ValueTask
// Solution: Convert to tasks or use manual iteration

// Option A: Convert to Task (loses some benefit)
await Task.WhenAll(tasks.Select(t => t.AsTask())).ConfigureAwait(false);

// Option B: Sequential await (simpler, different semantics)
foreach (var task in valueTasks)
{
    await task.ConfigureAwait(false);
}

// Option C: Use ValueTaskExtensions or custom helper
await ValueTaskExtensions.WhenAll(valueTasks).ConfigureAwait(false);
```

### Polyfill for netstandard2.0
ValueTask is available via `System.Threading.Tasks.Extensions` package for older frameworks.
Already referenced in `Directory.Packages.props` via `System.Threading.Channels`.

## 6. Testy do napisania/aktualizacji

| Typ | Projekt | Co testuje |
|-----|---------|------------|
| Unit | ReflectionEventing.UnitTests | EventBus with ValueTask returns |
| Unit | ReflectionEventing.UnitTests | IConsumer with ValueTask |
| Integration | All DI UnitTests | Verify ValueTask flows through DI |
| Performance | (New) | Benchmark Task vs ValueTask allocation |

## 7. Kolejność implementacji

1. [ ] Create ADR-005-valuetask-adoption.md
2. [ ] Update `IConsumer<T>` interface
3. [ ] Update `IEventsQueue` interface  
4. [ ] Update `IEventBus` interface
5. [ ] Update `EventsQueue` implementation
6. [ ] Update `EventBus` implementation (handle WhenAll issue)
7. [ ] Update `DependencyInjectionEventBus`
8. [ ] Update `DependencyInjectionQueueProcessor`
9. [ ] Update all test consumers in unit tests
10. [ ] Update Demo WPF consumers
11. [ ] Update `EventBusExtensions` (sync wrappers)
12. [ ] Run all tests
13. [ ] Update README.md examples
14. [ ] Update documentation
15. [ ] Bump version to 5.0.0 (major breaking change)

## 8. Ryzyka i decyzje

### Decisions Needed

- [ ] **WhenAll strategy**: How to handle parallel consumer execution?
  - Option A: Convert to Task (`.AsTask()`) - simple but allocates
  - Option B: Custom `ValueTask.WhenAll` helper
  - Option C: Change to sequential execution (different semantics!)
  
- [ ] **Version bump**: Major version required (4.x → 5.0.0)

- [ ] **Migration guide**: Need docs for users upgrading

### Risks

| Risk | Impact | Mitigation |
|------|--------|------------|
| Breaking all consumers | High | Clear migration guide, version bump |
| WhenAll complexity | Medium | Choose appropriate strategy |
| netstandard2.0 compatibility | Low | Already have polyfill package |
| State machine size increase | Low | Acceptable tradeoff |

## 9. Performance Expectations

| Scenario | Expected Improvement |
|----------|---------------------|
| Sync completion (no consumers) | Avoid Task allocation (~40-80 bytes) |
| Single consumer sync complete | Avoid Task allocation |
| Multiple consumers | Minimal (still need parallelization) |
| Queue operations | Avoid Task allocation on enqueue |

## 10. Aktualizacja dokumentacji

- [ ] Oznaczyć task jako done w `docs/board.md`
- [ ] Create ADR-005-valuetask-adoption.md
- [ ] Update README.md code examples
- [ ] Update docs/architecture/decisions/ 
- [ ] Add migration guide for v4 → v5
- [ ] Update CHANGELOG.md

## 11. Files to Modify (Complete List)

### Core Library
- `src/ReflectionEventing/IEventBus.cs`
- `src/ReflectionEventing/IConsumer.cs`
- `src/ReflectionEventing/EventBus.cs`
- `src/ReflectionEventing/EventBusExtensions.cs`
- `src/ReflectionEventing/Queues/IEventsQueue.cs`
- `src/ReflectionEventing/Queues/EventsQueue.cs`

### DI Integrations
- `src/ReflectionEventing.DependencyInjection/DependencyInjectionEventBus.cs`
- `src/ReflectionEventing.DependencyInjection/Services/DependencyInjectionQueueProcessor.cs`

### Demo
- `src/ReflectionEventing.Demo.Wpf/ViewModels/MainWindowViewModel.cs`
- `src/ReflectionEventing.Demo.Wpf/Services/BackgroundTickService.cs`

### Tests (all test consumers)
- `tests/ReflectionEventing.UnitTests/*.cs`
- `tests/ReflectionEventing.DependencyInjection.UnitTests/*.cs`
- `tests/ReflectionEventing.Autofac.UnitTests/*.cs`
- `tests/ReflectionEventing.Castle.Windsor.UnitTests/*.cs`
- `tests/ReflectionEventing.Ninject.UnitTests/*.cs`
- `tests/ReflectionEventing.Unity.UnitTests/*.cs`

### Documentation
- `README.md`
- `docs/architecture/decisions/ADR-005-valuetask-adoption.md` (new)
- `docs/board.md`

### Build
- `Directory.Build.props` (version bump to 5.0.0)
