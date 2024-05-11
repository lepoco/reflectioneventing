# 🚎 ReflectionEventing

[Created with ❤ in Poland by lepo.co](https://dev.lepo.co/)  
Unleash the power of decoupled design with eventing. ReflectionEventing empowers developers to create simple events between services using DI in WPF, WinForms, or CLI applications. By facilitating better Inversion of Control, ReflectionEventing helps reduce coupling, enhancing the modularity and flexibility of your applications.

[![GitHub license](https://img.shields.io/github/license/lepoco/reflectioneventing)](https://github.com/lepoco/reflectioneventing/blob/master/LICENSE) [![Nuget](https://img.shields.io/nuget/v/ReflectionEventing)](https://www.nuget.org/packages/ReflectionEventing/) [![Nuget](https://img.shields.io/nuget/dt/ReflectionEventing?label=nuget)](https://www.nuget.org/packages/ReflectionEventing/) [![Sponsors](https://img.shields.io/github/sponsors/lepoco)](https://github.com/sponsors/lepoco)

## 👀 What does this repo contain?

The repository contains NuGet package source code, which uses C# reflection to register services that can be used to listen for local events.

## Gettings started

ReflectionEventing is available as NuGet package on NuGet.org:  
https://www.nuget.org/packages/ReflectionEventing  
https://www.nuget.org/packages/ReflectionEventing.Autofac  
https://www.nuget.org/packages/ReflectionEventing.DependencyInjection

You can add it to your project using .NET CLI:

```powershell
dotnet add package ReflectionEventing.DependencyInjection
```

, or package manager console:

```powershell
NuGet\Install-Package ReflectionEventing.DependencyInjection
```

### Usage

#### Register consumers and bus

```csharp
IHost host = Host.CreateDefaultBuilder()
  .ConfigureServices((context, services) =>
    {
      _ = services.AddSingleton<MainWindowViewModel>();
      _ = services.AddEventBus(e =>
      {
        _ = e.AddConsumer<MainWindowViewModel>();
      });
    }
  )
  .Build();
```

#### Publish your event

```csharp
public class MyBackgroundService(IEventBus eventBus)
{
    public void PublishEvent()
    {
        eventBus.Publish(new BackgroundTicked());
    }
}
```

#### Now you can listen for the event

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

## Compilation

Use Visual Studio 2022 and invoke the .sln.

Visual Studio  
**ReflectionEventing** is an Open Source project. You are entitled to download and use the freely available Visual Studio Community Edition to build, run or develop for ReflectionEventing. As per the Visual Studio Community Edition license, this applies regardless of whether you are an individual or a corporate user.

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behavior in our community.

## License

**ReflectionEventing** is free and open source software licensed under **MIT License**. You can use it in private and commercial projects.  
Keep in mind that you must include a copy of the license in your project.
