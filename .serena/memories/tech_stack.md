# Tech Stack

## Programming Language
- C# 13.0
- Nullable reference types enabled

## Target Frameworks
- .NET 9.0
- .NET 8.0
- .NET 6.0
- .NET Standard 2.0
- .NET Framework 4.6.2
- .NET Framework 4.7.2

## Build System
- MSBuild
- .NET SDK 9.x
- Central Package Management (CPM) via Directory.Packages.props

## Testing
- xUnit 2.6.2
- NSubstitute 5.1.0 (mocking)
- AwesomeAssertions 8.0.1 (fluent assertions)
- coverlet.collector for code coverage

## Dependencies
- System.Threading.Channels (for queued event processing)
- System.Diagnostics.DiagnosticSource (for diagnostics)
- PolySharp (for polyfills on older frameworks)
- Microsoft.SourceLink.GitHub (for source linking)

## DI Frameworks Supported
- Microsoft.Extensions.DependencyInjection 8.0.0
- Autofac 4.0.0
- Castle.Windsor 6.0.0
- Ninject 3.0.1.10
- Unity 5.11.10

## Documentation
- DocFX for API documentation
- Custom templates with TypeScript/SCSS

## CI/CD
- GitHub Actions
- Windows runners
- Automated NuGet publishing
- Code signing with strong name keys

## Other Tools
- EditorConfig for code style enforcement
- StyleCop and .NET analyzers
