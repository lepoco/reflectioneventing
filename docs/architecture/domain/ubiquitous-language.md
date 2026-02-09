# Ubiquitous Language

This document defines the key terms and concepts used throughout the ReflectionEventing library.

## Core Terms

### Event
A message that represents something that has happened in the application. Events are immutable objects that carry data about the occurrence.

**Example:**
```csharp
public record BackgroundTicked(int Value);
```

### Event Bus
The central message broker that receives events from publishers and routes them to the appropriate consumers. It decouples the publisher from the consumers.

**In code:** `IEventBus`, `EventBus`

### Publisher
Any component that sends events to the event bus. Publishers don't know about consumers.

**Action:** Calls `IEventBus.SendAsync()` or `IEventBus.PublishAsync()`

### Consumer
A component that handles a specific type of event. Consumers implement `IConsumer<TEvent>` and process events asynchronously.

**In code:** `IConsumer<TEvent>`

**Example:**
```csharp
public class MyConsumer : IConsumer<BackgroundTicked>
{
    public ValueTask ConsumeAsync(BackgroundTicked payload, CancellationToken cancellationToken)
    {
        // Handle the event
        return ValueTask.CompletedTask;
    }
}
```

### Consumer Provider
An abstraction that resolves consumer instances from the dependency injection container. Each DI container has its own implementation.

**In code:** `IConsumerProvider`

### Consumer Types Provider
A component that maps event types to their registered consumer types. Uses hashing for O(1) lookup performance.

**In code:** `IConsumerTypesProvider`, `HashedConsumerTypesProvider`

### Events Queue
A channel-based queue for asynchronous event processing. Events published via `PublishAsync` are added to this queue for background processing.

**In code:** `IEventsQueue`, `EventsQueue`

## Operations

### Send
Immediately dispatches an event to all registered consumers within the current scope. Blocks until all consumers have completed processing.

**Method:** `IEventBus.SendAsync<TEvent>()`

**Characteristics:**
- Synchronous (waits for completion)
- Same scope as caller
- Exceptions propagate to caller

### Publish
Adds an event to the queue for background processing. Returns immediately without waiting for consumers.

**Method:** `IEventBus.PublishAsync<TEvent>()`

**Characteristics:**
- Asynchronous (fire-and-forget)
- Different scope (background processing)
- Exceptions are captured as `FailedEvent`

### Consume
The act of handling an event by a consumer. Each consumer processes the event in its `ConsumeAsync` method.

**Method:** `IConsumer<TEvent>.ConsumeAsync()`

## Patterns

### Observer Pattern
The underlying design pattern where the event bus acts as the subject and consumers are observers.

### Builder Pattern
Used for configuring the event bus. The `EventBusBuilder` provides a fluent API for registration.

**Example:**
```csharp
services.AddEventBus(builder =>
{
    builder.AddConsumer<MyConsumer>();
});
```

### Adapter Pattern
Each DI container integration is an adapter that implements the common `IConsumerProvider` interface.

## Components

| Term | Definition |
|------|------------|
| **EventBusBuilder** | Fluent builder for configuring event bus options and registering consumers |
| **EventBusBuilderOptions** | Configuration options for the event bus |
| **ProcessingMode** | Enum defining how events are processed (Sequential, Parallel, Queued) |
| **FailedEvent** | Wrapper for events that failed during processing, includes exception details |
| **EventBusException** | Exception thrown when event bus operations fail |
| **QueueException** | Exception thrown when queue operations fail |

## Naming Conventions

| Pattern | Convention | Example |
|---------|------------|---------|
| Event classes | Past tense or noun phrase | `BackgroundTicked`, `OrderCreated` |
| Consumer classes | `{Domain}{Action}Consumer` | `OrderCreatedConsumer` |
| Provider interfaces | `I{What}Provider` | `IConsumerProvider` |
| Builder classes | `{What}Builder` | `EventBusBuilder` |
| Extension classes | `{Type}Extensions` | `ServiceCollectionExtensions` |

## Acronyms

| Acronym | Meaning |
|---------|---------|
| **DI** | Dependency Injection |
| **IoC** | Inversion of Control |
| **AOT** | Ahead-of-Time (compilation) |

## See Also

- [Domain Overview](./overview.md) - Core concepts
- [Logical Architecture](../views/logical-architecture.md) - Component details

---

*Last updated: 2026-02-09*
