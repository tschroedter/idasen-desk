# Developer Guide

Welcome to the Idasen Desk Controller developer guide. This guide will help you set up your development environment and contribute to the project.

## Table of Contents

- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [Project Structure](#project-structure)
- [Building the Project](#building-the-project)
- [Running Tests](#running-tests)
- [Code Style](#code-style)
- [Contributing Guidelines](#contributing-guidelines)
- [Pull Request Process](#pull-request-process)
- [Debugging](#debugging)
- [Common Development Tasks](#common-development-tasks)

## Getting Started

### Prerequisites

Before you begin development, ensure you have:

- **Operating System**: Windows 10 or 11
- **IDE**: Visual Studio 2022 or VS Code with C# extension
- **.NET SDK**: .NET 8.0 SDK
- **Git**: For version control
- **GitHub CLI** (optional): For working with labels and PRs

### Recommended Tools

- **Visual Studio 2022** (Community, Professional, or Enterprise)
- **ReSharper** (optional, for enhanced C# development)
- **Git Bash** or Windows Terminal
- **SonarLint** (for real-time code quality feedback)

## Development Setup

### 1. Clone the Repository

```bash
# Clone the repository
git clone https://github.com/tschroedter/idasen-desk.git
cd idasen-desk
```

### 2. Install .NET SDK

Download and install [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

Verify installation:
```bash
dotnet --version
# Should show 8.0.x
```

### 3. Restore Dependencies

```bash
cd src
dotnet restore Idasen-Desk.sln
```

### 4. Open in IDE

**Visual Studio:**
```bash
start src/Idasen-Desk.sln
```

**VS Code:**
```bash
code .
```

## Project Structure

```
idasen-desk/
├── .github/                          # GitHub workflows and config
│   ├── workflows/                    # CI/CD pipelines
│   │   ├── dotnet-ci.yml            # Build, test, release
│   │   ├── release-drafter.yml      # Auto release notes
│   │   └── sonarcloud.yml           # Code quality
│   ├── copilot-instructions.md      # Development guidelines
│   └── pull_request_template.md     # PR template
├── docs/                             # Documentation
│   ├── CHANGELOG_AUTOMATION.md
│   ├── IMPLEMENTATION_SUMMARY.md
│   ├── SONARCLOUD_SETUP.md
│   ├── WORKFLOW_DIAGRAM.md
│   └── images/                       # Screenshot assets
├── scripts/                          # Helper scripts
│   └── create-labels.sh             # PR label setup
├── src/                              # Source code
│   ├── Idasen.SystemTray.Win11/     # Main application
│   │   ├── ViewModels/              # MVVM ViewModels
│   │   ├── Views/                   # WPF Views
│   │   ├── Services/                # Business logic services
│   │   ├── Helpers/                 # Helper classes
│   │   ├── Utils/                   # Utility classes
│   │   ├── App.xaml                 # Application definition
│   │   └── Program.cs               # Entry point
│   └── Idasen.SystemTray.Win11.Tests/ # Unit tests
│       ├── ViewModels/              # ViewModel tests
│       ├── Services/                # Service tests
│       └── Helpers/                 # Helper tests
├── wiki/                             # Wiki documentation
├── CHANGELOG.md                      # Auto-generated changelog
├── LICENSE                           # MIT License
└── README.md                         # Project readme
```

### Key Directories

- **`src/Idasen.SystemTray.Win11/`**: Main application code
- **`src/Idasen.SystemTray.Win11.Tests/`**: Unit tests
- **`.github/workflows/`**: CI/CD automation
- **`docs/`**: Project documentation

## Building the Project

### Command Line Build

```bash
# Navigate to source directory
cd src

# Debug build
dotnet build Idasen-Desk.sln --configuration Debug

# Release build
dotnet build Idasen-Desk.sln --configuration Release
```

### Build in Visual Studio

1. Open `Idasen-Desk.sln`
2. Select build configuration (Debug/Release)
3. Build → Build Solution (Ctrl+Shift+B)

### Build Output

Built files are located in:
```
src/Idasen.SystemTray.Win11/bin/[Debug|Release]/net8.0-windows10.0.19041/
```

### Publishing

To create a self-contained executable:

```bash
cd src
dotnet publish Idasen.SystemTray.Win11/Idasen.SystemTray.Win11.csproj \
  --configuration Release \
  --runtime win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishReadyToRun=true
```

Output: `src/Idasen.SystemTray.Win11/bin/Release/net8.0-windows10.0.19041/win-x64/publish/`

## Running Tests

### Command Line

```bash
cd src

# Run all tests
dotnet test Idasen-Desk.sln

# Run with detailed output
dotnet test Idasen-Desk.sln --verbosity normal

# Run specific test project
dotnet test Idasen.SystemTray.Win11.Tests/Idasen.SystemTray.Win11.Tests.csproj
```

### Visual Studio

1. Open Test Explorer (Test → Test Explorer)
2. Click "Run All Tests"
3. View results in Test Explorer window

### Test Framework

The project uses:
- **xUnit**: Testing framework
- **NSubstitute**: Mocking library
- **FluentAssertions**: Assertion library

### Writing Tests

Example test structure:

```csharp
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Idasen.SystemTray.Win11.Tests.Services
{
    public class MyServiceTests
    {
        [Fact]
        public void MethodName_Scenario_ExpectedBehavior()
        {
            // Arrange
            var dependency = Substitute.For<IDependency>();
            var sut = new MyService(dependency);

            // Act
            var result = sut.MethodToTest();

            // Assert
            result.Should().Be(expectedValue);
        }

        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(5, 5, 10)]
        public void Add_TwoNumbers_ReturnsSum(int a, int b, int expected)
        {
            // Arrange
            var calculator = new Calculator();

            // Act
            var result = calculator.Add(a, b);

            // Assert
            result.Should().Be(expected);
        }
    }
}
```

## Code Style

### General Guidelines

1. **Follow C# Conventions**
   - PascalCase for classes, methods, properties
   - camelCase for local variables, parameters
   - Prefix interfaces with `I`

2. **Code Analysis**
   - Project treats warnings as errors
   - .NET analyzers enabled
   - Code style enforced during build

3. **Nullable Reference Types**
   - Enabled throughout project
   - Always handle null cases
   - Use `?` for nullable types

### MVVM Pattern

The application follows MVVM architecture:

**Model**: Data and business logic
**View**: XAML UI components
**ViewModel**: Presentation logic and data binding

Key principles:
- ViewModels should be testable (no UI dependencies)
- Use interfaces for services
- Dependency injection via Autofac
- Commands for user interactions

### Naming Conventions

```csharp
// Classes
public class DeskController { }

// Interfaces
public interface IDeskService { }

// Methods
public void ConnectToDesk() { }

// Properties
public string DeskName { get; set; }

// Private fields
private readonly IDeskService _deskService;

// Constants
private const int MaxRetryAttempts = 3;
```

### Comments

- Add XML comments for public APIs
- Comment complex logic
- Don't state the obvious
- Keep comments up to date

```csharp
/// <summary>
/// Connects to the Idasen desk via Bluetooth LE.
/// </summary>
/// <param name="deskName">The name of the desk to connect to.</param>
/// <returns>True if connection successful, false otherwise.</returns>
public async Task<bool> ConnectAsync(string deskName)
{
    // Implementation
}
```

## Contributing Guidelines

### Before You Start

1. **Check Existing Issues**
   - Browse [open issues](https://github.com/tschroedter/idasen-desk/issues)
   - Avoid duplicate work
   - Comment if you plan to work on an issue

2. **Discuss Major Changes**
   - Open an issue for discussion first
   - Ensure alignment with project goals
   - Get feedback on approach

### Development Workflow

1. **Create a Branch**
   ```bash
   git checkout -b feature/my-feature
   # or
   git checkout -b fix/bug-description
   ```

2. **Make Changes**
   - Follow code style guidelines
   - Write/update tests
   - Keep commits focused and atomic

3. **Test Your Changes**
   - Run all tests locally
   - Test in actual application
   - Verify no regressions

4. **Commit Your Changes**
   ```bash
   git add .
   git commit -m "Add feature: brief description"
   ```

5. **Push and Create PR**
   ```bash
   git push origin feature/my-feature
   ```

## Pull Request Process

### PR Requirements

Before submitting a PR, ensure:

- ✅ Code follows project style
- ✅ All tests pass
- ✅ New tests added for new functionality
- ✅ Documentation updated if needed
- ✅ No new warnings introduced
- ✅ Self-review completed

### PR Labels

**Required**: Add at least one changelog label:

**Change Type Labels:**
- `feature` or `enhancement` - New features
- `fix`, `bugfix`, or `bug` - Bug fixes
- `chore` or `maintenance` - Maintenance tasks
- `documentation` or `docs` - Documentation changes
- `security` - Security-related changes

**Version Bump Labels** (optional, defaults to `patch`):
- `major` - Breaking changes (x.0.0)
- `minor` - New features (0.x.0)
- `patch` - Bug fixes (0.0.x)

### Creating PR Labels

If you're a maintainer:

```bash
# Authenticate with GitHub
gh auth login

# Run label creation script
./scripts/create-labels.sh
```

### PR Template

The repository includes a PR template. Fill out all sections:
- Description of changes
- Type of change
- Testing performed
- Checklist items

### Review Process

1. **Automated Checks**
   - CI/CD pipeline builds and tests
   - Code analysis runs
   - Must pass before merge

2. **Code Review**
   - Maintainers review code
   - Address feedback
   - Make requested changes

3. **Approval and Merge**
   - Once approved, maintainer merges
   - Changes appear in draft release
   - CHANGELOG updated on release

## Debugging

### Visual Studio Debugging

1. Set breakpoints in code
2. Press F5 to start debugging
3. Application launches with debugger attached

### Debug Output

Add debug output:

```csharp
System.Diagnostics.Debug.WriteLine($"Connecting to desk: {deskName}");
```

View in Visual Studio Output window (Debug → Windows → Output)

### Logging

The application uses structured logging:

```csharp
_logger.LogInformation("Connected to desk: {DeskName}", deskName);
_logger.LogWarning("Connection attempt {Attempt} failed", attemptNumber);
_logger.LogError(ex, "Failed to connect to desk");
```

Logs are written to files (location in Settings → Advanced).

### Bluetooth Debugging

For Bluetooth issues:
1. Use Windows Bluetooth Event Log
2. Check application logs
3. Use Bluetooth protocol analyzer (advanced)

## Common Development Tasks

### Adding a New Feature

1. **Create ViewModel** (if UI needed)
   ```csharp
   public class MyFeatureViewModel : ViewModelBase
   {
       // Implementation
   }
   ```

2. **Create View** (XAML)
   ```xml
   <UserControl x:Class="MyFeatureView">
       <!-- UI elements -->
   </UserControl>
   ```

3. **Register in IoC Container**
   ```csharp
   // In App.xaml.cs or module
   builder.RegisterType<MyFeatureViewModel>().AsSelf();
   ```

4. **Add Tests**
   ```csharp
   public class MyFeatureViewModelTests
   {
       // Test cases
   }
   ```

### Adding a New Service

1. **Define Interface**
   ```csharp
   public interface IMyService
   {
       Task DoSomethingAsync();
   }
   ```

2. **Implement Service**
   ```csharp
   public class MyService : IMyService
   {
       public async Task DoSomethingAsync()
       {
           // Implementation
       }
   }
   ```

3. **Register in IoC**
   ```csharp
   builder.RegisterType<MyService>().As<IMyService>().SingleInstance();
   ```

4. **Add Tests**
   ```csharp
   public class MyServiceTests
   {
       // Test cases with mocked dependencies
   }
   ```

### Updating Dependencies

1. **Check for Updates**
   ```bash
   dotnet list package --outdated
   ```

2. **Update Package**
   ```bash
   dotnet add package PackageName
   ```

3. **Test Thoroughly**
   - Run all tests
   - Test application manually
   - Check for breaking changes

4. **Update Documentation**
   - Note version changes in PR
   - Update relevant docs if API changed

### Adding Documentation

1. **Update Wiki** (in `wiki/` directory)
2. **Update README.md** (if public-facing change)
3. **Add XML comments** (for code documentation)
4. **Update CHANGELOG** (automatically via PR labels)

## Resources

### Documentation
- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [WPF Documentation](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [NSubstitute Documentation](https://nsubstitute.github.io/)

### Project Resources
- [GitHub Copilot Instructions](../.github/copilot-instructions.md)
- [Changelog Automation](../docs/CHANGELOG_AUTOMATION.md)
- [Implementation Summary](../docs/IMPLEMENTATION_SUMMARY.md)
- [Workflow Diagram](../docs/WORKFLOW_DIAGRAM.md)

### Community
- [GitHub Issues](https://github.com/tschroedter/idasen-desk/issues)
- [GitHub Discussions](https://github.com/tschroedter/idasen-desk/discussions)
- [Pull Requests](https://github.com/tschroedter/idasen-desk/pulls)

## Getting Help

If you need help:
1. Check existing documentation
2. Search [GitHub Issues](https://github.com/tschroedter/idasen-desk/issues)
3. Ask in [Discussions](https://github.com/tschroedter/idasen-desk/discussions)
4. Reach out to maintainers

---

**Navigation**: [Home](Home) | [Developer Guide](Developer-Guide) | [Build Instructions](Build-Instructions) | [Architecture](Architecture)
