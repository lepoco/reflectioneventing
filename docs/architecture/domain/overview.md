# Domain Overview

## Introduction

ReflectionEventing is a library, not a business application, so its "domain" consists of the patterns and concepts related to event-driven architecture and dependency injection.

## Core Domain Concepts

### Event Bus

The central component that enables decoupled communication between parts of an application through events.

```mermaid
classDiagram
    class IEventBus {
        <<interface>>
        +SendAsync~TEvent~(event, cancellationToken) Task
        +PublishAsync~TEvent~(event, cancellationToken) Task
    }
    
    class EventBus {
        -IConsumerProvider consumerProvider
        -IConsumerTypesProvider typesProvider
        -IEventsQueue queue
        +SendAsync~TEvent~(event, cancellationToken) Task
        +PublishAsync~TEvent~(event, cancellationToken) Task
    }
    
    IEventBus <|.. EventBus
```

### Consumer

A component that handles a specific type of event.

```mermaid
classDiagram
    class IConsumer~TEvent~ {
        <<interface>>
        +ConsumeAsync(payload, cancellationToken) Task
    }
    
    note for IConsumer~TEvent~ "Implemented by application code\nto handle specific event types"
```

### Consumer Provider

Abstracts the dependency injection container for resolving consumer instances.

```mermaid
classDiagram
    class IConsumerProvider {
        <<interface>>
        +GetConsumers(consumerType) IEnumerable~object~
    }
    
    class DependencyInjectionConsumerProvider
    class AutofacConsumerProvider
    class WindsorConsumerProvider
    class NinjectConsumerProvider
    class UnityConsumerProvider
    
    IConsumerProvider <|.. DependencyInjectionConsumerProvider
    IConsumerProvider <|.. AutofacConsumerProvider
    IConsumerProvider <|.. WindsorConsumerProvider
    IConsumerProvider <|.. NinjectConsumerProvider
    IConsumerProvider <|.. UnityConsumerProvider
```

### Consumer Types Provider

Maps event types to their registered consumer types.

```mermaid
classDiagram
    class IConsumerTypesProvider {
        <<interface>>
        +GetConsumerTypes(eventType) IEnumerable~Type~
    }
    
    class HashedConsumerTypesProvider {
        -Dictionary~Type,List~Type~~ consumerMap
        +GetConsumerTypes(eventType) IEnumerable~Type~
    }
    
    class HashedPolymorphicConsumerTypesProvider {
        -Dictionary~Type,List~Type~~ consumerMap
        +GetConsumerTypes(eventType) IEnumerable~Type~
    }
    
    IConsumerTypesProvider <|.. HashedConsumerTypesProvider
    IConsumerTypesProvider <|.. HashedPolymorphicConsumerTypesProvider
    
    note for HashedPolymorphicConsumerTypesProvider "Supports consuming events\nthrough base types/interfaces"
```

### Events Queue

A queue for asynchronous event processing.

```mermaid
classDiagram
    class IEventsQueue {
        <<interface>>
        +EnqueueAsync(event, cancellationToken) Task
        +DequeueAsync(cancellationToken) Task~object~
        +MarkAsCompletedAsync(event, cancellationToken) Task
        +MarkAsFailedAsync(event, exception, cancellationToken) Task
    }
    
    class EventsQueue {
        -Channel~object~ channel
        +EnqueueAsync(event, cancellationToken) Task
        +DequeueAsync(cancellationToken) Task~object~
    }
    
    class FailedEvent {
        +object Event
        +Exception Exception
        +DateTime FailedAt
    }
    
    IEventsQueue <|.. EventsQueue
    EventsQueue --> FailedEvent : tracks
```

## Domain Relationships

```mermaid
flowchart TD
    subgraph Publishing["Event Publishing"]
        Publisher[Publisher Component]
        Event[Event Object]
    end
    
    subgraph Bus["Event Bus"]
        IEventBus[IEventBus]
        EventBus[EventBus]
    end
    
    subgraph Routing["Consumer Routing"]
        TypesProvider[IConsumerTypesProvider]
        ConsumerProvider[IConsumerProvider]
    end
    
    subgraph Processing["Event Processing"]
        Queue[IEventsQueue]
        Consumer[IConsumer]
    end
    
    Publisher -->|creates| Event
    Publisher -->|sends via| IEventBus
    EventBus -->|queries| TypesProvider
    EventBus -->|resolves from| ConsumerProvider
    EventBus -->|may enqueue to| Queue
    ConsumerProvider -->|provides| Consumer
    Consumer -->|processes| Event
```

## Processing Modes

The library supports different processing modes for events:

| Mode | Method | Behavior |
|------|--------|----------|
| **Immediate** | `SendAsync` | Event is processed immediately in the current scope, blocking until all consumers complete |
| **Queued** | `PublishAsync` | Event is added to a queue for background processing, returns immediately |

## Key Design Decisions

1. **Interface-based design**: All core components are defined as interfaces, allowing for multiple implementations and easy testing.

2. **Generic events**: Events are typed using generics (`TEvent`), allowing compile-time type safety.

3. **Async-first**: All operations are asynchronous, supporting modern async/await patterns.

4. **DI-agnostic core**: The core library has no dependency on any specific DI container.

5. **Parallel processing**: Multiple consumers can process the same event in parallel.

## See Also

- [Ubiquitous Language](./ubiquitous-language.md) - Terminology definitions
- [Logical Architecture](../views/logical-architecture.md) - Component details
- [ADR-001](../decisions/ADR-001-event-bus-pattern.md) - Event Bus Pattern decision

---

*Last updated: 2026-02-09*
