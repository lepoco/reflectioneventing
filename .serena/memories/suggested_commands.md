# Suggested Commands

## Windows System Commands
Since this project is developed on Windows, use PowerShell commands:
- `Get-ChildItem` (or `dir`/`ls` alias) - List directory contents
- `Set-Location` (or `cd` alias) - Change directory
- `Select-String` - Search for text patterns (similar to grep)
- `Copy-Item` - Copy files
- `Remove-Item` - Delete files
- `New-Item` - Create files/directories

## .NET CLI Commands

### Restore Dependencies
```powershell
dotnet restore
```

### Build
```powershell
# Build entire solution
dotnet build ReflectionEventing.sln --configuration Release

# Build specific project
dotnet build src\ReflectionEventing\ReflectionEventing.csproj --configuration Release

# Build without restore
dotnet build ReflectionEventing.sln --configuration Release --no-restore
```

### Run Tests
```powershell
# Run all tests
dotnet test ReflectionEventing.sln --configuration Release

# Run tests without build
dotnet test ReflectionEventing.sln --configuration Release --no-restore --no-build

# Run tests with quiet verbosity (less output)
dotnet test ReflectionEventing.sln --configuration Release --verbosity quiet

# Run specific test project
dotnet test tests\ReflectionEventing.UnitTests\ReflectionEventing.UnitTests.csproj
```

### Create NuGet Packages
```powershell
# Packages are generated automatically on build if GeneratePackageOnBuild is true
dotnet build --configuration Release

# Or manually pack
dotnet pack src\ReflectionEventing\ReflectionEventing.csproj --configuration Release
```

### Clean Build Artifacts
```powershell
dotnet clean ReflectionEventing.sln --configuration Release
```

## Git Commands
```powershell
# Check status
git status

# View changes (without pager)
git --no-pager diff

# View specific file diff
git --no-pager diff -- src\ReflectionEventing\EventBus.cs

# Commit changes
git add .
git commit -m "Your message"

# Push changes
git push origin main
```

## NuGet Commands
```powershell
# Restore NuGet packages
nuget restore ReflectionEventing.sln

# Push package to NuGet.org
nuget push **\*.nupkg -Source 'https://api.nuget.org/v3/index.json'
```

## MSBuild Commands
```powershell
# Build with MSBuild
msbuild ReflectionEventing.sln /p:Configuration=Release

# Clean and rebuild
msbuild ReflectionEventing.sln /t:Clean,Build /p:Configuration=Release
```

## DocFX Commands
For building documentation:
```powershell
# Navigate to docs folder
cd docs

# Build documentation
docfx docfx.json
```

## Run Demo Application
```powershell
dotnet run --project src\ReflectionEventing.Demo.Wpf\ReflectionEventing.Demo.Wpf.csproj
```
