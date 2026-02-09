# ADR-001: Event Bus Pattern for Decoupled Communication

## Status

**Accepted**

## Date

2026-02-09

## Context

When building applications (WPF, WinForms, Console, or ASP.NET Core), components often need to communicate with each other. Traditional approaches include:

1. **Direct method calls** - Creates tight coupling between components
2. **Events/Delegates** - Better, but still requires references between components
3. **Mediator pattern** - Centralizes communication but can become complex

The project needs a communication mechanism that:
- Allows complete decoupling between publishers and subscribers
- Integrates seamlessly with dependency injection
- Supports asynchronous operations
- Is easy to test and maintain

## Decision

Implement an **Event Bus pattern** with the following characteristics:

1. **Central event bus (`IEventBus`)** that acts as a message broker
2. **Generic consumers (`IConsumer<TEvent>`)** for type-safe event handling
3. **Reflection-based discovery** of consumer registrations
4. **DI integration** for consumer resolution

### Key Design Elements

```csharp
public interface IEventBus
{
    Task SendAsync<TEvent>(TEvent eventItem, CancellationToken cancellationToken = default)
        where TEvent : class;
        
    Task PublishAsync<TEvent>(TEvent eventItem, CancellationToken cancellationToken = default)
        where TEvent : class;
}

public interface IConsumer<in TEvent>
{
    Task ConsumeAsync(TEvent payload, CancellationToken cancellationToken);
}
```

## Consequences

### Positive

- ✅ **Complete decoupling** - Publishers don't know about consumers
- ✅ **Testability** - Easy to mock the event bus in tests
- ✅ **Flexibility** - Multiple consumers can handle the same event
- ✅ **Type safety** - Generic consumers ensure compile-time type checking
- ✅ **Async-first** - Native support for async/await patterns
- ✅ **DI integration** - Consumers are resolved from the container

### Negative

- ⚠️ **Indirection** - Harder to trace event flow through the codebase
- ⚠️ **Runtime errors** - Consumer registration issues surface at runtime
- ⚠️ **Debugging complexity** - Stack traces span across the event bus

### Mitigations

- Use OpenTelemetry tracing for event flow visibility
- Add compile-time checks where possible
- Provide good error messages for misconfiguration

## Alternatives Considered

### 1. MediatR

**Pros:** Popular, well-documented, supports pipelines
**Cons:** Different design philosophy (request/response), external dependency

**Why rejected:** ReflectionEventing focuses on event-driven (fire-and-forget or parallel) scenarios rather than request/response.

### 2. .NET Events/Delegates

**Pros:** Built-in, simple, fast
**Cons:** Tight coupling, no DI integration, synchronous by default

**Why rejected:** Doesn't meet decoupling requirements.

### 3. Message Queue (RabbitMQ, Azure Service Bus)

**Pros:** Distributed, persistent, scalable
**Cons:** Infrastructure overhead, complexity for simple scenarios

**Why rejected:** Over-engineered for in-process communication; could be used alongside for distributed scenarios.

## Related Decisions

- [ADR-002](./ADR-002-multi-di-container-support.md) - Multi DI Container Support
- [ADR-003](./ADR-003-async-first-design.md) - Async-First Design

---

*Last updated: 2026-02-09*
