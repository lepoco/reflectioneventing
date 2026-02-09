# ReflectionEventing - Architecture Documentation

## Overview

ReflectionEventing is a powerful tool for developers looking to create decoupled designs in WPF, WinForms, or CLI applications. By leveraging the power of Dependency Injection (DI) and eventing, ReflectionEventing promotes better Inversion of Control (IoC), reducing coupling and enhancing the modularity and flexibility of your applications.

## Quick Navigation

| Section | Description |
|---------|-------------|
| [Views](./views/) | Architectural views (C4, logical, deployment) |
| [Domain](./domain/) | Domain model, bounded contexts, ubiquitous language |
| [Contracts](./contracts/) | API specifications, messaging, integration points |
| [Cross-cutting](./cross-cutting/) | Security, observability, scalability concerns |
| [Decisions](./decisions/) | Architecture Decision Records (ADRs) |
| [Appendix](./appendix/) | Diagrams, examples, reference materials |

## Architecture Summary

### Project Type

- **Languages**: C# (.NET 10, Framework 4.6.2/4.7.2)
- **Frameworks**: Microsoft.Extensions.DependencyInjection, Autofac, Castle Windsor, Ninject, Unity
- **Patterns**: Event Bus, Observer Pattern, Builder Pattern, Adapter Pattern

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                     Client Application                           │
│            (WPF, WinForms, Console, ASP.NET Core)               │
├─────────────────────────────────────────────────────────────────┤
│                        IEventBus                                 │
│                   ┌───────────────────┐                         │
│                   │    SendAsync()    │ ← Immediate execution   │
│                   │   PublishAsync()  │ ← Queued execution      │
│                   └───────────────────┘                         │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌──────────────────┐  ┌─────────────────┐    │
│  │  EventBus   │  │ ConsumerProvider │  │  EventsQueue    │    │
│  └─────────────┘  └──────────────────┘  └─────────────────┘    │
├─────────────────────────────────────────────────────────────────┤
│                    DI Container Adapters                         │
│  ┌─────────┐ ┌─────────┐ ┌────────┐ ┌────────┐ ┌───────────┐   │
│  │ MS.DI   │ │ Autofac │ │ Castle │ │Ninject │ │   Unity   │   │
│  └─────────┘ └─────────┘ └────────┘ └────────┘ └───────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

See [Context View](./views/context.md) for detailed system context.

## Module Structure

```
ReflectionEventing/
├── src/
│   ├── ReflectionEventing/           # Core library
│   │   ├── IEventBus.cs              # Main abstraction
│   │   ├── IConsumer.cs              # Consumer interface
│   │   ├── EventBus.cs               # Core implementation
│   │   ├── EventBusBuilder.cs        # Builder pattern
│   │   └── Queues/                   # Event queue support
│   ├── ReflectionEventing.DependencyInjection/   # MS DI integration
│   ├── ReflectionEventing.Autofac/               # Autofac integration
│   ├── ReflectionEventing.Castle.Windsor/        # Castle Windsor integration
│   ├── ReflectionEventing.Ninject/               # Ninject integration
│   └── ReflectionEventing.Unity/                 # Unity integration
└── tests/                            # Unit tests for all modules
```

## Key Architectural Decisions

| ADR | Title | Status |
|-----|-------|--------|
| [ADR-001](./decisions/ADR-001-event-bus-pattern.md) | Event Bus Pattern for Decoupled Communication | Accepted |
| [ADR-002](./decisions/ADR-002-multi-di-container-support.md) | Multi DI Container Support via Adapter Pattern | Accepted |
| [ADR-003](./decisions/ADR-003-async-first-design.md) | Async-First Design for All Operations | Accepted |
| [ADR-004](./decisions/ADR-004-multi-targeting.md) | Multi-targeting for Broad Framework Support | Accepted |

See [Decisions](./decisions/) for all Architecture Decision Records.

## Getting Started

### For New Team Members

1. Start with [Context View](./views/context.md) for system overview
2. Review [Domain Overview](./domain/overview.md) for business concepts
3. Check [Ubiquitous Language](./domain/ubiquitous-language.md) for terminology
4. Read key ADRs in [Decisions](./decisions/)

### For External Stakeholders

1. Review this README for high-level overview
2. Check [Context View](./views/context.md) for system boundaries
3. Review [API Contracts](./contracts/) for integration points

### For Development Team

1. Deep dive into [Logical Architecture](./views/logical-architecture.md)
2. Review [Domain Model](./domain/) for business rules
3. Check [Cross-cutting Concerns](./cross-cutting/) for technical requirements

## Core Concepts

### Event Bus

The central component that routes events to consumers. Supports two modes:

- **SendAsync**: Immediate, in-scope execution
- **PublishAsync**: Queued for background processing

### Consumer

Components that handle specific event types by implementing `IConsumer<TEvent>`.

### Consumer Provider

Abstracts the DI container to resolve consumer instances.

### Events Queue

Channel-based queue for background event processing.

---

## Contributing to Documentation

When updating this documentation:

1. **Keep diagrams in sync** with code changes
2. **Update ADRs** for significant architectural decisions
3. **Maintain ubiquitous language** when new domain terms emerge
4. **Version control** all documentation changes

## Tools Used

- **Diagrams**: Mermaid
- **Format**: Markdown
- **Generator**: arch-spec skill for Claude Code
