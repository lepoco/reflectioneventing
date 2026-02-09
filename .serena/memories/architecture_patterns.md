# Architecture and Design Patterns

## Core Architecture

ReflectionEventing follows a modular architecture with clear separation of concerns:

### Core Components

1. **EventBus** (`src/ReflectionEventing/EventBus.cs`)
   - Central event dispatching mechanism
   - Manages consumer registration and event publication
   - Supports multiple processing modes

2. **IConsumer&lt;T&gt;** Interface
   - Event consumers implement this interface
   - Provides `ConsumeAsync(T payload, CancellationToken)` method
   - Allows multiple consumers per event type

3. **Consumer Providers**
   - Abstract consumer resolution from specific DI containers
   - Each DI integration has its own provider implementation
   - Supports dependency injection for consumers

4. **Consumer Types Providers**
   - Hashed lookup for efficient consumer discovery
   - Supports polymorphic event handling
   - Uses reflection to find matching consumers

5. **Event Queues** (`src/ReflectionEventing/Queues/`)
   - Channel-based event queue implementation
   - Background processing support
   - Failed event handling

## Design Patterns

### Dependency Injection
- Core abstraction independent of specific DI container
- Adapter pattern for each DI framework
- Consumers resolved from DI container at runtime

### Builder Pattern
- `EventBusBuilder` for configuring the event bus
- Fluent API for adding consumers and options
- Extension methods for each DI integration

### Observer Pattern
- Event bus as subject
- Consumers as observers
- Decoupled event publishers and subscribers

### Strategy Pattern
- Different processing modes (sequential, parallel, queued)
- Pluggable consumer type resolution strategies
- Flexible error handling strategies

### Factory Pattern
- Consumer provider factories for each DI container
- EventBus instantiation through builders

## Processing Modes

1. **Sequential** - Events processed one at a time in order
2. **Parallel** - Multiple events processed concurrently
3. **Queued** - Events placed in queue for background processing

## Key Design Principles

### Inversion of Control (IoC)
- Event publishers don't know about consumers
- Consumers registered via DI container
- Loose coupling between components

### Single Responsibility
- EventBus: Event routing
- Consumer Providers: DI integration
- Consumer Types Providers: Type resolution
- Event Queues: Background processing

### Open/Closed Principle
- Core library closed for modification
- Open for extension via DI integrations
- Custom consumer types providers can be implemented

### Interface Segregation
- Small, focused interfaces (`IEventBus`, `IConsumer<T>`)
- Separate concerns (provider vs types provider)

### Dependency Inversion
- Depend on abstractions (`IEventBus`, `IConsumerProvider`)
- DI integrations depend on core abstractions

## Threading and Async
- Async/await throughout
- CancellationToken support
- Channel-based queuing for thread-safe event processing
- Task-based parallelism for concurrent event handling

## Error Handling
- EventBusException for bus-specific errors
- QueueException for queue-specific errors
- Failed events captured and can be reprocessed
- Graceful degradation on consumer failures

## Performance Considerations
- Hashed lookup for consumer types (O(1) average case)
- Channel-based queuing for efficient producer-consumer pattern
- AOT compilation support for .NET 8.0+
- Trimming support to reduce app size
