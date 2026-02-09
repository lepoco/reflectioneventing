# Logical Architecture View

> C4 Model - Level 2: Container Diagram & Level 3: Component Diagram

## Overview

This view presents the logical architecture of ReflectionEventing, showing the main modules (packages) and their components.

## Container Diagram (C4 Level 2)

```mermaid
C4Container
    title Container Diagram for ReflectionEventing
    
    Container_Boundary(core, "Core Library") {
        Container(eventBus, "ReflectionEventing", ".NET Library", "Core event bus implementation with interfaces and abstractions")
    }
    
    Container_Boundary(integrations, "DI Integrations") {
        Container(msDI, "ReflectionEventing.DependencyInjection", ".NET Library", "Microsoft.Extensions.DI integration")
        Container(autofac, "ReflectionEventing.Autofac", ".NET Library", "Autofac integration")
        Container(castle, "ReflectionEventing.Castle.Windsor", ".NET Library", "Castle Windsor integration")
        Container(ninject, "ReflectionEventing.Ninject", ".NET Library", "Ninject integration")
        Container(unity, "ReflectionEventing.Unity", ".NET Library", "Unity integration")
    }
    
    Rel(msDI, eventBus, "References")
    Rel(autofac, eventBus, "References")
    Rel(castle, eventBus, "References")
    Rel(ninject, eventBus, "References")
    Rel(unity, eventBus, "References")
```

## Module Overview

| Module | Technology | Purpose | Responsibility |
|--------|------------|---------|----------------|
| ReflectionEventing | .NET Standard 2.0+ | Core library | Event bus abstraction, interfaces, base implementation |
| ReflectionEventing.DependencyInjection | .NET Standard 2.0+ | MS DI adapter | Integration with Microsoft.Extensions.DependencyInjection |
| ReflectionEventing.Autofac | .NET Standard 2.0+ | Autofac adapter | Integration with Autofac IoC container |
| ReflectionEventing.Castle.Windsor | .NET Standard 2.0+ | Castle adapter | Integration with Castle Windsor container |
| ReflectionEventing.Ninject | .NET Standard 2.0+ | Ninject adapter | Integration with Ninject container |
| ReflectionEventing.Unity | .NET Standard 2.0+ | Unity adapter | Integration with Unity container |

## Core Component Diagram (C4 Level 3)

```mermaid
flowchart TB
    subgraph ReflectionEventing["ReflectionEventing (Core Library)"]
        IEventBus["IEventBus<br/>«interface»"]
        EventBus["EventBus<br/>«class»"]
        
        IConsumer["IConsumer&lt;TEvent&gt;<br/>«interface»"]
        
        IConsumerProvider["IConsumerProvider<br/>«interface»"]
        IConsumerTypesProvider["IConsumerTypesProvider<br/>«interface»"]
        
        HashedProvider["HashedConsumerTypesProvider<br/>«class»"]
        PolymorphicProvider["HashedPolymorphicConsumerTypesProvider<br/>«class»"]
        
        EventBusBuilder["EventBusBuilder<br/>«class»"]
        
        subgraph Queues["Queues"]
            IEventsQueue["IEventsQueue<br/>«interface»"]
            EventsQueue["EventsQueue<br/>«class»"]
            FailedEvent["FailedEvent<br/>«class»"]
        end
        
        EventBus -->|implements| IEventBus
        EventBus -->|uses| IConsumerProvider
        EventBus -->|uses| IConsumerTypesProvider
        EventBus -->|uses| IEventsQueue
        
        HashedProvider -->|implements| IConsumerTypesProvider
        PolymorphicProvider -->|implements| IConsumerTypesProvider
        
        EventsQueue -->|implements| IEventsQueue
        EventsQueue -->|stores| FailedEvent
        
        EventBusBuilder -->|creates| EventBus
    end
```

## Component Responsibilities

| Component | Responsibility | Dependencies |
|-----------|----------------|--------------|
| `IEventBus` | Contract for event publishing | None |
| `EventBus` | Routes events to consumers | IConsumerProvider, IConsumerTypesProvider, IEventsQueue |
| `IConsumer<TEvent>` | Contract for event consumption | None |
| `IConsumerProvider` | Resolves consumer instances from DI | None |
| `IConsumerTypesProvider` | Maps event types to consumer types | None |
| `HashedConsumerTypesProvider` | O(1) lookup for consumer types | None |
| `HashedPolymorphicConsumerTypesProvider` | Supports polymorphic event handling | None |
| `EventBusBuilder` | Fluent API for configuration | All above |
| `IEventsQueue` | Contract for event queuing | None |
| `EventsQueue` | Channel-based event queue | System.Threading.Channels |
| `FailedEvent` | Tracks failed event processing | None |

## DI Integration Pattern

Each DI integration package follows the same pattern:

```mermaid
flowchart LR
    subgraph DIPackage["DI Integration Package"]
        ContainerAdapter["ConsumerProvider<br/>«class»"]
        BuilderExt["EventBusBuilder<br/>«class»"]
        Extensions["ContainerExtensions<br/>«static class»"]
    end
    
    subgraph Core["ReflectionEventing Core"]
        IConsumerProvider2["IConsumerProvider"]
        EventBusBuilder2["EventBusBuilder"]
    end
    
    subgraph DIContainer["DI Container"]
        Container["IContainer/IServiceProvider"]
    end
    
    ContainerAdapter -->|implements| IConsumerProvider2
    BuilderExt -->|extends| EventBusBuilder2
    ContainerAdapter -->|uses| Container
    Extensions -->|configures| Container
```

## Module Structure

```
ReflectionEventing/
├── src/
│   ├── ReflectionEventing/
│   │   ├── IEventBus.cs
│   │   ├── EventBus.cs
│   │   ├── IConsumer.cs
│   │   ├── IConsumerProvider.cs
│   │   ├── IConsumerTypesProvider.cs
│   │   ├── HashedConsumerTypesProvider.cs
│   │   ├── HashedPolymorphicConsumerTypesProvider.cs
│   │   ├── EventBusBuilder.cs
│   │   ├── EventBusBuilderOptions.cs
│   │   ├── EventBusException.cs
│   │   ├── ProcessingMode.cs
│   │   └── Queues/
│   │       ├── IEventsQueue.cs
│   │       ├── EventsQueue.cs
│   │       ├── FailedEvent.cs
│   │       └── QueueException.cs
│   │
│   ├── ReflectionEventing.DependencyInjection/
│   │   ├── DependencyInjectionConsumerProvider.cs
│   │   ├── DependencyInjectionEventBus.cs
│   │   ├── DependencyInjectionEventBusBuilder.cs
│   │   ├── ServiceCollectionExtensions.cs
│   │   ├── EventBusBuilderExtensions.cs
│   │   └── Configuration/
│   │       └── QueueProcessorOptionsProvider.cs
│   │
│   ├── ReflectionEventing.Autofac/
│   │   ├── AutofacConsumerProvider.cs
│   │   ├── AutofacEventBusBuilder.cs
│   │   └── ContainerBuilderExtensions.cs
│   │
│   ├── ReflectionEventing.Castle.Windsor/
│   │   ├── WindsorConsumerProvider.cs
│   │   ├── WindsorEventBusBuilder.cs
│   │   └── EventBusInstaller.cs
│   │
│   ├── ReflectionEventing.Ninject/
│   │   ├── NinjectConsumerProvider.cs
│   │   ├── NinjectEventBusBuilder.cs
│   │   └── EventBusModule.cs
│   │
│   └── ReflectionEventing.Unity/
│       ├── UnityConsumerProvider.cs
│       ├── UnityEventBusBuilder.cs
│       └── UnityContainerExtensions.cs
│
└── tests/
    ├── ReflectionEventing.UnitTests/
    ├── ReflectionEventing.DependencyInjection.UnitTests/
    ├── ReflectionEventing.Autofac.UnitTests/
    ├── ReflectionEventing.Castle.Windsor.UnitTests/
    ├── ReflectionEventing.Ninject.UnitTests/
    └── ReflectionEventing.Unity.UnitTests/
```

## Module Dependencies

```mermaid
flowchart TD
    Core[ReflectionEventing]
    
    MSDI[ReflectionEventing.DependencyInjection]
    Autofac[ReflectionEventing.Autofac]
    Castle[ReflectionEventing.Castle.Windsor]
    Ninject[ReflectionEventing.Ninject]
    Unity[ReflectionEventing.Unity]
    
    MSDI --> Core
    Autofac --> Core
    Castle --> Core
    Ninject --> Core
    Unity --> Core
    
    subgraph External["External Dependencies"]
        MSExtDI["Microsoft.Extensions.DI"]
        AutofacLib["Autofac"]
        CastleLib["Castle.Windsor"]
        NinjectLib["Ninject"]
        UnityLib["Unity"]
    end
    
    MSDI --> MSExtDI
    Autofac --> AutofacLib
    Castle --> CastleLib
    Ninject --> NinjectLib
    Unity --> UnityLib
```

## Communication Patterns

### Event Flow - SendAsync (Synchronous)

```mermaid
sequenceDiagram
    participant Publisher
    participant EventBus
    participant TypesProvider as ConsumerTypesProvider
    participant ConsumerProvider
    participant Consumer1 as Consumer 1
    participant Consumer2 as Consumer 2
    
    Publisher->>EventBus: SendAsync(event)
    EventBus->>TypesProvider: GetConsumerTypes(eventType)
    TypesProvider-->>EventBus: [Consumer1Type, Consumer2Type]
    
    EventBus->>ConsumerProvider: GetConsumers(Consumer1Type)
    ConsumerProvider-->>EventBus: [consumer1Instance]
    
    EventBus->>ConsumerProvider: GetConsumers(Consumer2Type)
    ConsumerProvider-->>EventBus: [consumer2Instance]
    
    par Parallel Execution
        EventBus->>Consumer1: ConsumeAsync(event)
        EventBus->>Consumer2: ConsumeAsync(event)
    end
    
    Consumer1-->>EventBus: Task completed
    Consumer2-->>EventBus: Task completed
    
    EventBus-->>Publisher: Task completed
```

### Event Flow - PublishAsync (Queued)

```mermaid
sequenceDiagram
    participant Publisher
    participant EventBus
    participant Queue as EventsQueue
    participant Processor as QueueProcessor
    participant Consumer
    
    Publisher->>EventBus: PublishAsync(event)
    EventBus->>Queue: EnqueueAsync(event)
    Queue-->>EventBus: Task completed
    EventBus-->>Publisher: Task completed (event queued)
    
    Note over Processor: Background processing
    
    Processor->>Queue: DequeueAsync()
    Queue-->>Processor: event
    Processor->>Consumer: ConsumeAsync(event)
    Consumer-->>Processor: Task completed
```

## Technology Stack

### Runtime

| Component | Technology | Version |
|-----------|------------|---------|
| Runtime | .NET 9/8/6/Standard 2.0/FX 4.6.2+ | Multiple |
| Language | C# | 13.0 |
| Async | System.Threading.Channels | 9.0.0 |
| Observability | System.Diagnostics.DiagnosticSource | 6.0.0 |

### Development

| Tool | Purpose |
|------|---------|
| xUnit | Unit testing |
| NSubstitute | Mocking |
| AwesomeAssertions | Fluent assertions |
| StyleCop | Code style enforcement |
| PolySharp | Polyfills for older frameworks |

## See Also

- [Context View](./context.md) - System context
- [Domain Model](../domain/overview.md) - Business domain
- [ADR-002](../decisions/ADR-002-multi-di-container-support.md) - Multi DI Container decision

---

*Last updated: 2026-02-09*
