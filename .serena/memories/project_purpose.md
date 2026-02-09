# Project Purpose

## ReflectionEventing

ReflectionEventing is a powerful tool for developers looking to create decoupled designs in WPF, WinForms, or CLI applications. By leveraging the power of Dependency Injection (DI) and eventing, ReflectionEventing promotes better Inversion of Control (IoC), reducing coupling and enhancing the modularity and flexibility of your applications.

## Key Features

- Event bus implementation with reflection-based consumer registration
- Support for multiple DI containers:
  - Microsoft.Extensions.DependencyInjection
  - Autofac
  - Castle Windsor
  - Ninject
  - Unity
- Async event consumption
- Multiple processing modes (sequential, parallel, queued)
- Multi-targeting support (net9.0, net8.0, net6.0, netstandard2.0, net462, net472)
- AOT compilation support for .NET 8.0+

## Project Structure

- `src/ReflectionEventing` - Core library with EventBus implementation
- `src/ReflectionEventing.DependencyInjection` - Microsoft DI integration
- `src/ReflectionEventing.Autofac` - Autofac integration
- `src/ReflectionEventing.Castle.Windsor` - Castle Windsor integration
- `src/ReflectionEventing.Ninject` - Ninject integration
- `src/ReflectionEventing.Unity` - Unity integration
- `src/ReflectionEventing.Demo.Wpf` - WPF demo application
- `tests/` - Unit tests for all packages
- `docs/` - Documentation and DocFX templates

## Distribution

All packages are distributed via NuGet.org with package names:
- ReflectionEventing
- ReflectionEventing.DependencyInjection
- ReflectionEventing.Autofac
- ReflectionEventing.Castle.Windsor
- ReflectionEventing.Ninject
- ReflectionEventing.Unity
