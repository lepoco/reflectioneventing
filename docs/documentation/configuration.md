# EventBusBuilder Configuration

The `EventBusBuilder` class is used to configure and build an event bus with a specific set of consumers. This document provides an overview of the configuration options and usage examples.

## Configuration Options

### EventBusBuilderOptions

The `EventBusBuilderOptions` class provides several configuration options to control the behavior of the event bus:

- **UseEventPolymorphism**: Indicates whether the event bus should use event polymorphism. If set to `true`, the event bus will deliver events to consumers that handle the event type or any of its base types. The default value is `false`.

- **UseEventsQueue**: Indicates whether the event bus should use a background events queue. If set to `true`, the event bus will use a background queue to process events. The default value is `true`.

- **UseErrorQueue**: Indicates whether the event bus should use an error queue. If set to `true`, the event bus will use an error queue to handle events that fail processing. The default value is `false`.

- **QueueTickRate**: The rate at which the event queue is processed. The default value is 20ms.

- **ErrorTickRate**: The rate at which the error queue is processed when default queue consumption fails. The default value is 20ms.

## Usage Examples

### Adding Consumers

You can add consumers to the `EventBusBuilder` using the `AddConsumer` method. This method checks if the consumer is registered in the service collection and if it is not transient. It then gets the interfaces of the consumer that are generic and have a generic type definition of `IConsumer<TEvent>`. For each of these interfaces, it gets the generic argument and adds it to the consumers dictionary.

```csharp
services.AddEventBus(builder =>
{
    builder.Options.UseEventPolymorphism = true;
    builder.AddConsumer(typeof(MyConsumer));
});
```
