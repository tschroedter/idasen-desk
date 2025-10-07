# Copilot Instructions for idasen-desk

## Project Overview

This repository contains a Windows 10/11 desktop application for controlling Ikea's Idasen standing desk via Bluetooth LE. The application provides a system tray interface with features like preset heights, hotkey support, and automatic connection management.

**Technology Stack:**
- C# / .NET 8.0
- WPF (Windows Presentation Foundation)
- Target Framework: `net8.0-windows10.0.19041`
- Dependency Injection: Autofac
- Bluetooth LE for desk communication
- Testing: xUnit, NSubstitute, FluentAssertions

## Repository Structure

```
├── .github/                    # GitHub workflows and configurations
│   ├── workflows/             # CI/CD pipelines
│   │   ├── dotnet-ci.yml     # Build, test, and release workflow
│   │   ├── release-drafter.yml # Automated release notes
│   │   └── update-changelog.yml # Changelog updates
│   ├── release-drafter.yml    # Release drafter configuration
│   └── pull_request_template.md
├── docs/                       # Documentation
│   ├── CHANGELOG_AUTOMATION.md
│   ├── IMPLEMENTATION_SUMMARY.md
│   └── README.md
├── scripts/                    # Helper scripts
│   └── create-labels.sh       # PR label creation script
├── src/                        # Source code
│   ├── Idasen.SystemTray.Win11/        # Main application
│   │   ├── ViewModels/        # MVVM ViewModels
│   │   ├── Views/             # WPF Views
│   │   ├── Services/          # Application services
│   │   ├── Helpers/           # Helper classes
│   │   └── Utils/             # Utility classes
│   └── Idasen.SystemTray.Win11.Tests/  # Unit tests
└── CHANGELOG.md               # Auto-generated changelog
```

## Building and Testing

### Prerequisites
- .NET 8.0 SDK
- Windows 10/11 (for building and running)
- Visual Studio 2022 or VS Code with C# extension

### Build Commands

```bash
# Navigate to source directory
cd src

# Restore dependencies
dotnet restore Idasen-Desk.sln

# Build solution (Release)
dotnet build Idasen-Desk.sln --configuration Release

# Build solution (Debug)
dotnet build Idasen-Desk.sln --configuration Debug

# Run tests
dotnet test Idasen-Desk.sln --configuration Release --verbosity normal

# Publish self-contained executable
dotnet publish Idasen.SystemTray.Win11/Idasen.SystemTray.Win11.csproj \
  --configuration Release \
  --runtime win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishReadyToRun=true
```

### Running Tests

All tests use xUnit framework and should pass before submitting PRs:

```bash
cd src
dotnet test --no-build --verbosity normal
```

## Code Style and Standards

### General Guidelines

1. **Warnings as Errors**: The project treats warnings as errors (`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`)
2. **Nullable Reference Types**: Enabled throughout the project
3. **Code Analysis**: 
   - .NET analyzers are enabled with `latest-recommended` analysis level
   - Code style is enforced during build
   - Analyzers run during both build and live analysis

### Naming Conventions

- Follow standard C# naming conventions
- Use PascalCase for classes, methods, and properties
- Use camelCase for local variables and parameters
- Prefix interfaces with `I` (e.g., `ISettingsService`)
- Use meaningful, descriptive names

### Testing Guidelines

- Test files mirror the structure of the main project
- Use xUnit for test framework
- Use NSubstitute for mocking
- Use FluentAssertions for assertions
- Test class names should end with `Tests`
- Test method names should clearly describe what is being tested (underscore separation is allowed for tests - see `NoWarn CA1707`)

### Code Organization

- Follow MVVM pattern for UI components
- Use dependency injection via Autofac
- Keep ViewModels testable and UI-framework independent
- Services should be registered in the IoC container
- Use interfaces for testability and loose coupling

## Pull Request Guidelines

### Required PR Labels

All PRs **must** have at least one changelog label. This is crucial for our automated changelog system:

**Change Type Labels** (choose at least one):
- `feature` or `enhancement` - New features and enhancements
- `fix`, `bugfix`, or `bug` - Bug fixes
- `chore` or `maintenance` - Maintenance tasks, refactoring, dependency updates
- `documentation` or `docs` - Documentation changes
- `security` - Security-related changes

**Version Bump Labels** (optional, defaults to `patch`):
- `major` - Breaking changes (x.0.0)
- `minor` - New features (0.x.0)
- `patch` - Bug fixes (0.0.x) - **default**

### PR Checklist

Before submitting a PR, ensure:
- [ ] Code follows the project's code style
- [ ] Self-review completed
- [ ] Code is commented where necessary (especially complex logic)
- [ ] Documentation updated if applicable
- [ ] No new warnings introduced
- [ ] Tests added/updated for new functionality
- [ ] All tests pass locally
- [ ] At least one changelog label added

### Setting Up PR Labels

If you're a maintainer and need to create the required labels:

```bash
# Install GitHub CLI if needed
gh auth login

# Run the label creation script
./scripts/create-labels.sh
```

## Changelog Automation

This repository uses automated changelog generation:

1. **Release Drafter**: Automatically creates/updates draft releases based on merged PRs
2. **Changelog Updater**: Updates CHANGELOG.md when a release is published

### Workflow
1. Create a PR with your changes
2. Add appropriate labels (see above)
3. Merge to `main` branch
4. Changes automatically appear in draft release
5. Maintainer publishes release
6. CHANGELOG.md automatically updates

For detailed information, see [docs/CHANGELOG_AUTOMATION.md](../docs/CHANGELOG_AUTOMATION.md).

## Development Workflow

### Working on Features

1. Create a feature branch from `main`
2. Make your changes following the code style guidelines
3. Write/update tests for your changes
4. Run tests locally to ensure they pass
5. Build the solution to verify no warnings/errors
6. Create a PR with appropriate labels
7. Address any CI/CD failures or review feedback

### CI/CD Pipeline

The `dotnet-ci.yml` workflow runs on:
- All pushes to `main`
- All pull requests targeting `main`
- Manual trigger via workflow_dispatch

It performs:
1. Checkout and setup .NET 8.0
2. Version stamping (from git tags or run number)
3. Restore NuGet packages
4. Build the solution
5. Run all tests
6. Publish self-contained executable
7. Create GitHub release (draft)
8. Tag the commit with version

## Common Tasks

### Adding a New Feature

1. Create necessary ViewModels, Views, or Services
2. Register new services in the IoC container (typically in `App.xaml.cs` or a module)
3. Write unit tests for business logic
4. Update documentation if the feature affects user interaction
5. Add `feature` or `enhancement` label to PR

### Fixing a Bug

1. Add a test that reproduces the bug (if possible)
2. Fix the issue
3. Verify the test passes
4. Add `fix`, `bugfix`, or `bug` label to PR

### Updating Dependencies

1. Update NuGet package versions in `.csproj` files
2. Build and test thoroughly
3. Check for any breaking changes in updated packages
4. Add `chore` or `maintenance` label to PR

### Adding Tests

1. Mirror the structure of the main project
2. Use xUnit's `[Fact]` or `[Theory]` attributes
3. Use NSubstitute for mocking dependencies
4. Use FluentAssertions for readable assertions
5. Follow the Arrange-Act-Assert pattern

Example test structure:
```csharp
[Fact]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange
    var dependency = Substitute.For<IDependency>();
    var sut = new SystemUnderTest(dependency);

    // Act
    var result = sut.MethodToTest();

    // Assert
    result.Should().Be(expectedValue);
}
```

## Documentation

- Main README: Project overview, installation, features
- `docs/CHANGELOG_AUTOMATION.md`: Detailed guide on changelog system
- `docs/IMPLEMENTATION_SUMMARY.md`: Implementation details and setup
- `docs/WORKFLOW_DIAGRAM.md`: Visual representation of the workflow
- Inline code comments: For complex logic and non-obvious behavior

## Resources

- [Keep a Changelog](https://keepachangelog.com/) - Changelog format
- [Semantic Versioning](https://semver.org/) - Version numbering
- [xUnit Documentation](https://xunit.net/) - Testing framework
- [FluentAssertions](https://fluentassertions.com/) - Assertion library
- [NSubstitute](https://nsubstitute.github.io/) - Mocking framework
- [WPF Documentation](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/) - UI framework

## Support

For questions or issues:
- Check existing documentation in `docs/`
- Review workflow runs in the Actions tab
- Create an issue with appropriate labels
- Refer to PR template for contribution guidelines

---

**Last Updated**: October 2024
**Maintainer**: tschroedter
