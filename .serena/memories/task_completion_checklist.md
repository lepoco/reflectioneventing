# Task Completion Checklist

When completing a development task in ReflectionEventing, follow these steps:

## 1. Code Changes
- [ ] Make the necessary code changes
- [ ] Ensure file headers are present and correct in all modified files
- [ ] Use proper naming conventions (see code_style_and_conventions.md)
- [ ] Follow C# coding standards defined in .editorconfig
- [ ] Add XML documentation comments for public APIs
- [ ] Use file-scoped namespaces
- [ ] Place using directives outside namespaces

## 2. Build
```powershell
dotnet build ReflectionEventing.sln --configuration Release --no-restore
```
- [ ] Verify build succeeds without errors
- [ ] Check for any new analyzer warnings
- [ ] Ensure no StyleCop violations

## 3. Run Tests
```powershell
dotnet test ReflectionEventing.sln --configuration Release --no-restore --no-build
```
- [ ] Verify all existing tests pass
- [ ] Add new tests for new functionality
- [ ] Use xUnit, NSubstitute for mocks, AwesomeAssertions for assertions
- [ ] Ensure test coverage for edge cases

## 4. Code Quality
- [ ] Review code for potential null reference issues
- [ ] Ensure async methods have 'Async' suffix
- [ ] Verify CancellationToken usage in async methods
- [ ] Check for proper disposal of IDisposable resources
- [ ] Ensure thread-safety where necessary

## 5. Multi-Targeting Considerations
- [ ] If changes affect compatibility, test on multiple frameworks:
  - net9.0, net8.0, net6.0, netstandard2.0, net462, net472
- [ ] Use conditional compilation if needed (#if NET8_0_OR_GREATER)
- [ ] Check PolySharp polyfills work for older frameworks

## 6. Documentation
- [ ] Update README.md if public API changes
- [ ] Update docs/documentation/*.md if behavior changes
- [ ] Ensure XML documentation is complete for public members

## 7. Git Workflow
```powershell
# Check what changed
git status
git --no-pager diff

# Stage changes
git add .

# Commit with descriptive message
git commit -m "Description of changes"

# Push to branch (not directly to main)
git push origin your-branch-name
```

## 8. Before Creating PR
- [ ] Rebase on latest main if needed
- [ ] Run full build and test suite one more time
- [ ] Check that no unnecessary files are included
- [ ] Write descriptive PR title and description
- [ ] Reference any related issues

## Notes
- **Do not push directly to main** - use pull requests
- **Do not commit secrets** or credentials
- PR validator will run automatically and check:
  - Build success
  - All tests passing
  - No StyleCop violations
