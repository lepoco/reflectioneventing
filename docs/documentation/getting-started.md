# Getting Started with Reflection Eventing

Welcome to the "Getting Started" guide for the Reflection Eventing library. This guide will walk you through the initial setup and basic usage of the library.

## Prerequisites

Before you begin, ensure you have the following:

- .NET 5.0 or later installed on your machine.
- A basic understanding of dependency injection and asynchronous programming in .NET.

## Installation

To install the Reflection Eventing library, add the following NuGet package to your project:

```bash
dotnet add package ReflectionEventing
```

## Basic Setup

#### Step 1: Define Your Events and Consumers

First, define the events and consumers that will handle those events. An event is typically a simple class or record, and a consumer is a class that implements the IConsumer<TEvent> interface.

```csharp
public record TestEvent;

public class TestConsumer : IConsumer<TestEvent>
{
    public ValueTask ConsumeAsync(TestEvent payload, CancellationToken cancellationToken)
    {
        // Handle the event
        return ValueTask.CompletedTask;
    }
}
```

#### Step 2: Configure the Event Bus

Next, configure the event bus in your application's dependency injection container. You can do this in the ConfigureServices method of your Startup class or in the Host.CreateDefaultBuilder method.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReflectionEventing;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddEventBus(builder =>
                {
                    builder.Options.UseEventPolymorphism = true;
                    builder.AddConsumer<TestConsumer>();
                });
            });
}
```

#### Step 3: Publish Events

To publish events, resolve the IEventBus service from the dependency injection container and call the PublishAsync method.

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReflectionEventing;

public class EventPublisher
{
    private readonly IEventBus _eventBus;

    public EventPublisher(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task PublishTestEventAsync()
    {
        await _eventBus.PublishAsync(new TestEvent());
    }
}
```

#### Step 4: Run Your Application

Run your application and ensure that the events are being published and consumed as expected.
