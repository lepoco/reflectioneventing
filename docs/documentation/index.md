# Reflection Eventing Docs

[Created with ❤ in Poland by lepo.co](https://dev.lepo.co/)  
ReflectionEventing is a powerful tool for developers looking to create decoupled designs in WPF, WinForms, or CLI applications. By leveraging the power of Dependency Injection (DI) and eventing, ReflectionEventing promotes better Inversion of Control (IoC), reducing coupling and enhancing the modularity and flexibility of your applications.

[![GitHub license](https://img.shields.io/github/license/lepoco/reflectioneventing)](https://github.com/lepoco/reflectioneventing/blob/master/LICENSE) [![Nuget](https://img.shields.io/nuget/v/ReflectionEventing)](https://www.nuget.org/packages/ReflectionEventing/) [![Nuget](https://img.shields.io/nuget/dt/ReflectionEventing?label=nuget)](https://www.nuget.org/packages/ReflectionEventing/) [![Sponsors](https://img.shields.io/github/sponsors/lepoco)](https://github.com/sponsors/lepoco)

## 👀 What does this repo contain?

This repository houses the source code for the ReflectionEventing NuGet package. The package utilizes C# reflection to register services that can listen for and respond to local events.

## 🛟 Support plans

To ensure you receive the expert guidance you need, we offer a variety of support plans designed to meet the diverse needs of our community. Whether you are looking to in memory eventing or need assistance with our other libraries, our tailored support solutions are here to help. From priority email support to 24/7 dedicated assistance, we provide flexible plans to suit your project requirements.

[Take a look at the lepo.co support plans](https://lepo.co/support)

## 🤝 Help us keep working on this project

Support the development of Reflection Eventing and other innovative projects by becoming a sponsor on GitHub! Your monthly or one-time contributions help us continue to deliver high-quality, open-source solutions that empower developers worldwide.

[Sponsor Reflection Eventing on GitHub](https://github.com/sponsors/lepoco)

## Gettings started

ReflectionEventing is available as NuGet package on NuGet.org:  
<https://www.nuget.org/packages/ReflectionEventing>  
<https://www.nuget.org/packages/ReflectionEventing.Autofac>  
<https://www.nuget.org/packages/ReflectionEventing.Castle.Windsor>  
<https://www.nuget.org/packages/ReflectionEventing.DependencyInjection>  
<https://www.nuget.org/packages/ReflectionEventing.Ninject>  
<https://www.nuget.org/packages/ReflectionEventing.Unity>

You can add it to your project using .NET CLI:

```powershell
dotnet add package ReflectionEventing.DependencyInjection
```

, or package manager console:

```powershell
NuGet\Install-Package ReflectionEventing.DependencyInjection
```

### 🛠️ How to Use ReflectionEventing

#### 1. Register Consumers and the Event Bus

In this step, we register our ViewModel as a singleton and add it as a consumer to the event bus. This allows the ViewModel to listen for events published on the bus.

```csharp
IHost host = Host.CreateDefaultBuilder()
  .ConfigureServices((context, services) =>
    {
      services.AddSingleton<MainWindowViewModel>();
      services.AddEventBus(e =>
      {
        e.AddConsumer<MainWindowViewModel>();
      });
    }
  )
  .Build();
```

#### 2. Publish Events

Here, we create a background service that publishes an event on the event bus. This event could be anything - in this case, we're publishing a `BackgroundTicked` event.

```csharp
public class MyBackgroundService(IEventBus eventBus)
{
    public void PublishEvent()
    {
        eventBus.Publish(new BackgroundTicked());
    }
}
```

#### 3. Listen for Events

Finally, we implement the `IConsumer<T>` interface in our ViewModel. This allows the ViewModel to consume `BackgroundTicked` events. When a `BackgroundTicked` event is published, the `ConsumeAsync` method is called, and we update the `CurrentTick` property.

```csharp
public partial class MainWindowViewModel : ObservableObject, IConsumer<BackgroundTicked>
{
    [ObservableProperty]
    private int _currentTick = 0;

    public Task ConsumeAsync(BackgroundTicked payload, CancellationToken cancellationToken)
    {
        CurrentTick = payload.Value;

        return Task.CompletedTask;
    }
}
```

## Special thanks

JetBrains was kind enough to lend a license for the open-source **dotUltimate** for _ReflectionEventing_ development.  
Learn more here:

- <https://www.jetbrains.com/dotnet/>
- <https://www.jetbrains.com/opensource/>

## Compilation

To build the project, use Visual Studio 2022 and open the .sln file.

Visual Studio  
**ReflectionEventing** is an Open Source project. You are entitled to download and use the freely available Visual Studio Community Edition to build, run or develop for ReflectionEventing. As per the Visual Studio Community Edition license, this applies regardless of whether you are an individual or a corporate user.

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behavior in our community.

## License

**ReflectionEventing** is free and open source software licensed under **MIT License**. You can use it in private and commercial projects.  
Keep in mind that you must include a copy of the license in your project.
