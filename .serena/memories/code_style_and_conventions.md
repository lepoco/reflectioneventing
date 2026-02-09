# Code Style and Conventions

## File Header
All C# source files must include the following header:
```
// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and ReflectionEventing Contributors.
// All Rights Reserved.
```

## Encoding and Indentation
- UTF-8 encoding
- 4 spaces for indentation (not tabs)
- Tab width: 4
- End of line: unset (cross-platform)

## Naming Conventions
- **Classes, structs, enums, delegates**: PascalCase
- **Interfaces**: PascalCase with 'I' prefix (e.g., `IEventBus`)
- **Methods, properties, events**: PascalCase
- **Public/internal fields**: PascalCase
- **Private/protected fields**: camelCase with underscore prefix (e.g., `_fieldName`)
- **Parameters**: camelCase
- **Local variables**: camelCase
- **Const fields**: PascalCase
- **Static readonly fields**: PascalCase
- **Private readonly fields**: camelCase
- **Async methods**: Should end with 'Async' suffix

## C# Style Preferences
- **var**: Avoid using `var` (`false:warning` for non-apparent types)
- **this.**: Don't use `this.` qualifier unless necessary
- **Language keywords**: Prefer language keywords over BCL types (e.g., `int` not `Int32`)
- **Expression bodies**: Prefer block bodies for methods, constructors, accessors
- **Braces**: Always use braces for control flow statements
- **Namespaces**: Use file-scoped namespaces
- **using directives**: Place outside namespace
- **Nullable**: Enable nullable reference types
- **Pattern matching**: Prefer pattern matching over `as` with null check
- **Target-typed new**: Use target-typed `new()` expressions

## Code Analysis
- EnforceCodeStyleInBuild: true
- StyleCop analyzers enabled (with some rules suppressed)
- .NET code analysis warnings enabled
- File header template enforced (IDE0073: error)

## Documentation
- XML documentation required for public APIs in core projects
- Use `///` for XML doc comments
- Tests and demo projects don't require documentation

## Unsafe Code
- Allowed when necessary (`AllowUnsafeBlocks: true`)
- CS8500 warnings suppressed in unsafe contexts

## GlobalUsings
Each project should have a `GlobalUsings.cs` file with common imports:
- System namespaces
- Testing frameworks (xUnit, NSubstitute, FluentAssertions for tests)
- ReflectionEventing namespaces

## Async/Await
- Always use async/await for asynchronous operations
- Pass CancellationToken parameters where appropriate
- Return Task/Task&lt;T&gt; for async methods
