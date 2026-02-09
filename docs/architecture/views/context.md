# System Context View

> C4 Model - Level 1: System Context Diagram

## Overview

This view shows the ReflectionEventing library in the context of its environment, including the applications that use it and the DI containers it integrates with.

## Context Diagram

```mermaid
C4Context
    title System Context Diagram for ReflectionEventing
    
    Person(developer, "Developer", "Application developer using the library")
    
    System_Boundary(app, "Client Application") {
        System(clientApp, "Client Application", "WPF, WinForms, Console, or ASP.NET Core application")
    }
    
    System(reflectionEventing, "ReflectionEventing", "Event bus library for decoupled communication via DI and reflection")
    
    System_Ext(msDI, "Microsoft.Extensions.DependencyInjection", "Microsoft DI container")
    System_Ext(autofac, "Autofac", "Autofac IoC container")
    System_Ext(castle, "Castle Windsor", "Castle Windsor IoC container")
    System_Ext(ninject, "Ninject", "Ninject IoC container")
    System_Ext(unity, "Unity", "Unity IoC container")
    System_Ext(nuget, "NuGet.org", "Package distribution")
    
    Rel(developer, clientApp, "Builds")
    Rel(clientApp, reflectionEventing, "Uses")
    Rel(reflectionEventing, msDI, "Integrates with")
    Rel(reflectionEventing, autofac, "Integrates with")
    Rel(reflectionEventing, castle, "Integrates with")
    Rel(reflectionEventing, ninject, "Integrates with")
    Rel(reflectionEventing, unity, "Integrates with")
    Rel(nuget, reflectionEventing, "Distributes")
```

## System Description

| Element | Type | Description |
|---------|------|-------------|
| ReflectionEventing | Library | Core event bus library enabling decoupled pub/sub communication |
| Client Application | System | Any .NET application consuming the library |
| DI Containers | External Systems | IoC containers for dependency resolution |
| NuGet.org | Distribution | Package hosting and distribution |

## Users and Actors

| Actor | Description | Interactions |
|-------|-------------|--------------|
| Developer | Application developer | Integrates library, registers consumers, publishes events |
| Consumer | Event handler component | Receives and processes events |
| Publisher | Event source component | Creates and publishes events |

## Integration Points

### Supported DI Containers

| Container | Package | Minimum Version |
|-----------|---------|-----------------|
| Microsoft.Extensions.DependencyInjection | ReflectionEventing.DependencyInjection | 3.1.0+ |
| Autofac | ReflectionEventing.Autofac | 4.0.0+ |
| Castle Windsor | ReflectionEventing.Castle.Windsor | 6.0.0+ |
| Ninject | ReflectionEventing.Ninject | 3.0.1+ |
| Unity | ReflectionEventing.Unity | 5.11.0+ |

### Target Frameworks

| Framework | Support Level |
|-----------|--------------|
| .NET 9.0 | Full (AOT enabled) |
| .NET 8.0 | Full (AOT enabled) |
| .NET 6.0 | Full |
| .NET Standard 2.0 | Full |
| .NET Framework 4.6.2 | Full |
| .NET Framework 4.7.2 | Full |

## System Boundaries

### What's Inside the Boundary

- Event bus core implementation
- Consumer discovery and registration
- Event routing and dispatching
- Queue-based async event processing
- DI container adapters
- Observability (OpenTelemetry metrics and traces)

### What's Outside the Boundary

- DI container implementations (provided by external libraries)
- Application-specific event types
- Consumer implementations
- Application hosting and lifecycle

## Usage Pattern

```mermaid
sequenceDiagram
    participant App as Application
    participant DI as DI Container
    participant Bus as IEventBus
    participant Provider as ConsumerProvider
    participant Consumer as IConsumer<T>
    
    App->>DI: Register services & consumers
    App->>DI: AddEventBus()
    
    Note over App,Consumer: At runtime
    
    App->>Bus: SendAsync(event)
    Bus->>Provider: GetConsumerTypes(eventType)
    Provider->>DI: Resolve consumers
    DI-->>Provider: Consumer instances
    Bus->>Consumer: ConsumeAsync(event)
    Consumer-->>Bus: Task completed
    Bus-->>App: Task completed
```

## Quality Attributes

| Attribute | Requirement | How Addressed |
|-----------|-------------|---------------|
| Availability | High (library shouldn't crash host) | Exception handling, CancellationToken support |
| Scalability | Handle many events efficiently | Parallel processing, Channel-based queuing |
| Performance | Low latency event dispatch | Hashed consumer lookup (O(1)), async/await |
| Extensibility | Support multiple DI containers | Adapter pattern, interface-based design |
| Testability | Easy to mock and test | Interface-based design, NSubstitute support |

## See Also

- [Logical Architecture](./logical-architecture.md) - Component details
- [Domain Overview](../domain/overview.md) - Core concepts
- [ADR-001](../decisions/ADR-001-event-bus-pattern.md) - Event Bus Pattern decision

---

*Last updated: 2026-02-09*
