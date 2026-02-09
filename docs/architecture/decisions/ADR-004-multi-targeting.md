# ADR-004: Multi-targeting for Broad Framework Support

## Status

**Accepted**

## Date

2026-02-09

## Context

.NET ecosystem has evolved significantly:

- **.NET Framework 4.6.2+** - Still used in enterprise WPF/WinForms
- **.NET Standard 2.0** - Bridge between old and new
- **.NET 6.0** - LTS, widely adopted
- **.NET 8.0** - Current LTS, AOT support
- **.NET 9.0** - Latest, cutting edge features

ReflectionEventing targets:
- WPF applications (often .NET Framework or .NET 6+)
- WinForms applications (legacy .NET Framework)
- Console/CLI tools (modern .NET)
- ASP.NET Core services (.NET 6+)

To maximize adoption, the library must support multiple target frameworks.

## Decision

Implement **multi-targeting** with the following targets:

| Target Framework | Reason |
|------------------|--------|
| `net9.0` | Latest features, best performance, AOT |
| `net8.0` | Current LTS, AOT support |
| `net6.0` | Previous LTS, wide adoption |
| `netstandard2.0` | .NET Framework compatibility |
| `net462` | Legacy WPF/WinForms support |
| `net472` | Legacy with some modern APIs |

### Project File Configuration

```xml
<PropertyGroup>
  <TargetFrameworks>net9.0;net8.0;net6.0;netstandard2.0;net462;net472</TargetFrameworks>
  <LangVersion>13.0</LangVersion>
  <Nullable>enable</Nullable>
</PropertyGroup>

<PropertyGroup Condition="'$(IsBelowNet8)' == 'false'">
  <PublishAot>true</PublishAot>
  <IsTrimmable>true</IsTrimmable>
</PropertyGroup>
```

### Polyfill Strategy

Use **PolySharp** to provide missing APIs on older frameworks:

```xml
<ItemGroup>
  <PackageReference Include="PolySharp">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>build; analyzers</IncludeAssets>
  </PackageReference>
</ItemGroup>
```

Polyfilled types include:
- `CallerArgumentExpressionAttribute`
- `IsExternalInit` (for records)
- `RequiredMemberAttribute`
- Nullable analysis attributes

## Consequences

### Positive

- ✅ **Broad adoption** - Works in legacy and modern projects
- ✅ **Future-proof** - Ready for .NET 9 and beyond
- ✅ **AOT support** - Native compilation for .NET 8+
- ✅ **Trimming** - Smaller deployments on modern frameworks
- ✅ **Single codebase** - Minimal conditional compilation

### Negative

- ⚠️ **Complexity** - Must test on all targets
- ⚠️ **Feature restrictions** - Can't use APIs not available everywhere
- ⚠️ **Package size** - Multiple TFMs increase NuGet package size
- ⚠️ **Polyfill dependencies** - Additional build-time dependency

### Mitigations

- Automated CI builds on all targets
- Conditional dependencies where needed
- Clear documentation of framework-specific behavior

## Framework-Specific Dependencies

```xml
<ItemGroup Condition="'$(TargetFramework)' == 'netstandard2.0' or 
                       '$(TargetFramework)' == 'net462' or 
                       '$(TargetFramework)' == 'net472'">
  <PackageReference Include="System.Threading.Channels" />
  <PackageReference Include="System.Diagnostics.DiagnosticSource" />
</ItemGroup>
```

## Conditional Compilation

When framework-specific code is needed:

```csharp
#if NET8_0_OR_GREATER
    // Use modern APIs
    ReadOnlySpan<char> span = value.AsSpan();
#else
    // Fallback for older frameworks
    string span = value;
#endif
```

## Alternatives Considered

### 1. .NET Standard 2.0 Only

**Pros:** Single target, simpler
**Cons:** No AOT, no trimming, misses modern optimizations

**Why rejected:** Leaves significant value on the table for modern apps.

### 2. Modern .NET Only (.NET 6+)

**Pros:** Modern APIs, simpler testing
**Cons:** Excludes legacy WPF/WinForms apps

**Why rejected:** Significant user base on .NET Framework.

### 3. Separate Packages per Target

**Pros:** Cleaner separation
**Cons:** Maintenance overhead, confusing for users

**Why rejected:** Multi-targeting in single package is standard practice.

## Build Matrix

| Framework | Windows | Linux | macOS | AOT |
|-----------|---------|-------|-------|-----|
| net9.0 | ✅ | ✅ | ✅ | ✅ |
| net8.0 | ✅ | ✅ | ✅ | ✅ |
| net6.0 | ✅ | ✅ | ✅ | ❌ |
| netstandard2.0 | ✅ | ✅ | ✅ | ❌ |
| net462 | ✅ | ❌ | ❌ | ❌ |
| net472 | ✅ | ❌ | ❌ | ❌ |

## Related Decisions

- [ADR-002](./ADR-002-multi-di-container-support.md) - Multi DI Container Support

---

*Last updated: 2026-02-09*
