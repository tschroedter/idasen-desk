# Testing Guide

Guide for writing and running tests in the Idasen Desk Controller project.

## Overview

The project uses comprehensive unit testing to ensure code quality, maintainability, and correctness. This guide covers the testing approach, tools, and best practices.

## Testing Framework

### Core Technologies

- **xUnit** - Test framework
- **NSubstitute** - Mocking library
- **FluentAssertions** - Assertion library

### Why These Tools?

**xUnit**
- Modern, widely-adopted testing framework
- Excellent Visual Studio integration
- Supports async tests
- Extensible and maintainable

**NSubstitute**
- Clean, fluent API
- Easy to create and configure mocks
- Powerful verification capabilities
- Great for interface-based testing

**FluentAssertions**
- Readable assertions
- Detailed error messages
- Extensive assertion methods
- Better test maintainability

## Test Structure

### Test Project Organization

```
Idasen.SystemTray.Win11.Tests/
├── ViewModels/
│   ├── MainViewModelTests.cs
│   ├── SettingsViewModelTests.cs
│   └── ...
├── Services/
│   ├── DeskServiceTests.cs
│   ├── SettingsServiceTests.cs
│   └── ...
├── Helpers/
│   ├── HotkeyManagerTests.cs
│   └── ...
└── Utils/
    ├── ConverterTests.cs
    └── ...
```

**Mirror main project structure** - Tests are organized to match the source code structure.

### Test File Naming

- Test files end with `Tests.cs`
- Match the class being tested: `DeskService.cs` → `DeskServiceTests.cs`

### Test Method Naming

Use descriptive names that explain what's being tested:

```csharp
[Fact]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Test implementation
}
```

**Examples:**
```csharp
ConnectAsync_WhenDeskNotPaired_ReturnsFalse()
MoveToHeight_WithValidHeight_SendsCorrectCommand()
SaveSettings_WhenFileAccessDenied_ThrowsException()
```

## Writing Tests

### Basic Test Structure (AAA Pattern)

**Arrange-Act-Assert** pattern for clear, maintainable tests:

```csharp
[Fact]
public void Add_TwoNumbers_ReturnsSum()
{
    // Arrange - Set up test data and dependencies
    var calculator = new Calculator();
    var a = 5;
    var b = 3;

    // Act - Execute the method being tested
    var result = calculator.Add(a, b);

    // Assert - Verify the expected outcome
    result.Should().Be(8);
}
```

### Testing with Dependencies

Use NSubstitute to mock dependencies:

```csharp
[Fact]
public async Task ConnectAsync_WhenSuccessful_RaisesConnectionEvent()
{
    // Arrange
    var bluetoothService = Substitute.For<IBluetoothService>();
    bluetoothService.ConnectAsync().Returns(Task.FromResult(true));
    
    var logger = Substitute.For<ILogger<DeskService>>();
    var sut = new DeskService(bluetoothService, logger);

    var eventRaised = false;
    sut.ConnectionStatusChanged += (s, e) => eventRaised = true;

    // Act
    await sut.ConnectAsync();

    // Assert
    eventRaised.Should().BeTrue();
    await bluetoothService.Received(1).ConnectAsync();
}
```

### Testing Async Methods

Use `async`/`await` in test methods:

```csharp
[Fact]
public async Task MoveToHeightAsync_WithValidHeight_CompletesSuccessfully()
{
    // Arrange
    var service = CreateDeskService();

    // Act
    var result = await service.MoveToHeightAsync(120);

    // Assert
    result.Should().BeTrue();
}
```

### Testing Exceptions

```csharp
[Fact]
public void Constructor_WithNullDependency_ThrowsArgumentNullException()
{
    // Arrange & Act
    Action act = () => new DeskService(null);

    // Assert
    act.Should().Throw<ArgumentNullException>()
       .WithParameterName("bluetoothService");
}
```

### Testing Events

```csharp
[Fact]
public void HeightChanged_WhenHeightUpdates_RaisesEvent()
{
    // Arrange
    var service = CreateDeskService();
    var heightReceived = 0;
    service.HeightChanged += (s, height) => heightReceived = height;

    // Act
    service.UpdateHeight(120);

    // Assert
    heightReceived.Should().Be(120);
}
```

## Test Categories

### Unit Tests

Test individual components in isolation:

```csharp
[Fact]
public void ValidateHeight_WithValidValue_ReturnsTrue()
{
    // Arrange
    var validator = new HeightValidator();

    // Act
    var result = validator.Validate(100);

    // Assert
    result.Should().BeTrue();
}
```

### Testing ViewModels

ViewModels should be testable without UI dependencies:

```csharp
[Fact]
public void ConnectCommand_WhenExecuted_CallsDeskService()
{
    // Arrange
    var deskService = Substitute.For<IDeskService>();
    var viewModel = new MainViewModel(deskService);

    // Act
    viewModel.ConnectCommand.Execute(null);

    // Assert
    deskService.Received(1).ConnectAsync();
}
```

### Testing Services

Test business logic and coordination:

```csharp
[Fact]
public async Task SaveSettings_WithValidSettings_WritesToFile()
{
    // Arrange
    var fileService = Substitute.For<IFileService>();
    var settingsService = new SettingsService(fileService);
    var settings = new Settings { /* ... */ };

    // Act
    await settingsService.SaveAsync(settings);

    // Assert
    await fileService.Received(1).WriteTextAsync(
        Arg.Any<string>(), 
        Arg.Is<string>(s => s.Contains("Standing")));
}
```

## FluentAssertions Examples

### Basic Assertions

```csharp
// Equality
result.Should().Be(expected);
result.Should().NotBe(unexpected);

// Nullability
result.Should().BeNull();
result.Should().NotBeNull();

// Boolean
condition.Should().BeTrue();
condition.Should().BeFalse();
```

### String Assertions

```csharp
text.Should().StartWith("Hello");
text.Should().EndWith("World");
text.Should().Contain("middle");
text.Should().BeEmpty();
text.Should().NotBeNullOrWhiteSpace();
```

### Collection Assertions

```csharp
collection.Should().HaveCount(5);
collection.Should().Contain(item);
collection.Should().ContainSingle(x => x.Id == 1);
collection.Should().BeEmpty();
collection.Should().OnlyContain(x => x.IsValid);
```

### Exception Assertions

```csharp
action.Should().Throw<InvalidOperationException>()
      .WithMessage("Invalid state");

action.Should().NotThrow();

async () => await service.MethodAsync()
    .Should().ThrowAsync<TimeoutException>();
```

### Numeric Assertions

```csharp
value.Should().BeGreaterThan(5);
value.Should().BeLessThan(10);
value.Should().BeInRange(5, 10);
value.Should().BeCloseTo(3.14, 0.01);
```

## NSubstitute Examples

### Creating Mocks

```csharp
// Interface mock
var service = Substitute.For<IService>();

// Multiple interfaces
var mock = Substitute.For<IService, IDisposable>();

// Partial mock (concrete class)
var partial = Substitute.ForPartsOf<ConcreteClass>();
```

### Configuring Return Values

```csharp
// Simple return value
service.GetValue().Returns(42);

// Conditional returns
service.GetValue(1).Returns(10);
service.GetValue(2).Returns(20);

// Callback with return
service.Calculate(Arg.Any<int>())
       .Returns(x => (int)x[0] * 2);
```

### Async Methods

```csharp
// Return Task
service.ProcessAsync().Returns(Task.CompletedTask);

// Return Task<T>
service.GetDataAsync().Returns(Task.FromResult(data));

// Using ReturnsForAnyArgs
service.LoadAsync(Arg.Any<string>())
       .Returns(Task.FromResult(result));
```

### Verifying Calls

```csharp
// Verify called once
service.Received(1).Method();

// Verify called with specific arguments
service.Received().Method(Arg.Is<int>(x => x > 5));

// Verify not called
service.DidNotReceive().Method();

// Verify call order
Received.InOrder(() =>
{
    service.First();
    service.Second();
});
```

### Argument Matching

```csharp
// Any argument
service.Method(Arg.Any<int>());

// Specific value
service.Method(Arg.Is(42));

// Conditional
service.Method(Arg.Is<int>(x => x > 10));

// Type check
service.Method(Arg.Is<object>(x => x is string));
```

## Test Data

### Using Theories

Test multiple scenarios with `[Theory]`:

```csharp
[Theory]
[InlineData(60, true)]
[InlineData(130, true)]
[InlineData(50, false)]
[InlineData(140, false)]
public void ValidateHeight_VariousValues_ReturnsExpected(
    int height, 
    bool expected)
{
    // Arrange
    var validator = new HeightValidator();

    // Act
    var result = validator.Validate(height);

    // Assert
    result.Should().Be(expected);
}
```

### Using MemberData

For complex test data:

```csharp
public static IEnumerable<object[]> GetTestData()
{
    yield return new object[] { 60, 130, 95 };
    yield return new object[] { 70, 120, 95 };
    yield return new object[] { 80, 110, 95 };
}

[Theory]
[MemberData(nameof(GetTestData))]
public void Calculate_WithRanges_ReturnsMiddle(
    int min, 
    int max, 
    int expected)
{
    // Test implementation
}
```

## Running Tests

### Command Line

```bash
# Run all tests
dotnet test

# Run with verbosity
dotnet test --verbosity normal

# Run specific test
dotnet test --filter "FullyQualifiedName~DeskServiceTests"

# Run tests in parallel
dotnet test --parallel
```

### Visual Studio

1. **Test Explorer**
   - View → Test Explorer
   - Click "Run All Tests"
   - Or right-click specific tests

2. **Keyboard Shortcuts**
   - `Ctrl + R, A` - Run all tests
   - `Ctrl + R, T` - Run tests in current context
   - `Ctrl + R, Ctrl + T` - Debug tests in context

### VS Code

1. Install ".NET Core Test Explorer" extension
2. Tests appear in sidebar
3. Click play button to run tests

## Code Coverage

### Collecting Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Coverage report generated in:
```
TestResults/[guid]/coverage.cobertura.xml
```

### Viewing Coverage

**Using VS Enterprise:**
- Analyze → Code Coverage

**Using Report Generator:**
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool

reportgenerator \
  -reports:"TestResults/**/coverage.cobertura.xml" \
  -targetdir:"coveragereport" \
  -reporttypes:Html
```

### Coverage Goals

- **Minimum**: 70% overall coverage
- **Target**: 80%+ for critical paths
- **ViewModels**: 90%+ (high business logic)
- **Services**: 85%+ (core functionality)
- **Utilities**: 80%+ (reusable components)

## Best Practices

### DO:

✅ **Test public behavior**, not implementation details
✅ **Use descriptive test names** that explain what's being tested
✅ **Follow AAA pattern** (Arrange, Act, Assert)
✅ **Test edge cases** and error conditions
✅ **Keep tests independent** - no shared state
✅ **Use appropriate assertion methods** from FluentAssertions
✅ **Mock external dependencies** for unit tests
✅ **Test one thing per test** - single assertion focus

### DON'T:

❌ **Don't test framework code** (e.g., WPF binding)
❌ **Don't test private methods** directly
❌ **Don't share state** between tests
❌ **Don't use Thread.Sleep** for async tests
❌ **Don't catch exceptions** without re-throwing
❌ **Don't skip tests** without good reason
❌ **Don't test implementation details** that may change

## Common Testing Patterns

### Testing Commands

```csharp
[Fact]
public void Command_WhenCanExecute_ExecutesAction()
{
    // Arrange
    var service = Substitute.For<IService>();
    var viewModel = new MyViewModel(service);
    
    // Act - Check CanExecute
    var canExecute = viewModel.MyCommand.CanExecute(null);
    
    // Act - Execute
    viewModel.MyCommand.Execute(null);
    
    // Assert
    canExecute.Should().BeTrue();
    service.Received(1).DoSomething();
}
```

### Testing Property Changed

```csharp
[Fact]
public void Property_WhenSet_RaisesPropertyChanged()
{
    // Arrange
    var viewModel = new MyViewModel();
    var propertyChanged = false;
    viewModel.PropertyChanged += (s, e) =>
    {
        if (e.PropertyName == nameof(MyViewModel.MyProperty))
            propertyChanged = true;
    };

    // Act
    viewModel.MyProperty = "new value";

    // Assert
    propertyChanged.Should().BeTrue();
}
```

### Testing Async void Event Handlers

```csharp
[Fact]
public async Task EventHandler_WhenRaised_PerformsAction()
{
    // Arrange
    var service = Substitute.For<IService>();
    var viewModel = new MyViewModel(service);
    var tcs = new TaskCompletionSource<bool>();
    
    service.When(x => x.DoSomethingAsync())
           .Do(_ => tcs.SetResult(true));

    // Act
    viewModel.HandleEvent(this, EventArgs.Empty);

    // Assert
    await tcs.Task.WithTimeout(TimeSpan.FromSeconds(1));
}
```

## Debugging Tests

### Visual Studio

1. Set breakpoint in test
2. Right-click test → Debug Test
3. Step through code

### Viewing Test Output

```csharp
[Fact]
public void Test_WithOutput()
{
    // Use output helper (inject ITestOutputHelper)
    _output.WriteLine("Debug information");
}
```

### Isolating Failing Tests

```bash
# Run only failed tests
dotnet test --filter "FullyQualifiedName~FailingTest"
```

## Continuous Integration

Tests run automatically on:
- Every push to main
- All pull requests
- Manual workflow triggers

See `.github/workflows/dotnet-ci.yml`

## Troubleshooting Tests

### Tests Pass Locally But Fail in CI

- **Timing issues**: Use proper async patterns, not Thread.Sleep
- **Environment differences**: Check file paths, OS-specific code
- **Resource cleanup**: Ensure proper disposal

### Flaky Tests

- **Race conditions**: Fix async/await usage
- **Shared state**: Ensure test isolation
- **External dependencies**: Mock properly

### Slow Tests

- **Mock expensive operations**: Don't hit real databases/files
- **Optimize setup**: Use constructor for common setup
- **Parallel execution**: Enable if tests are independent

## Related Resources

- [xUnit Documentation](https://xunit.net/)
- [NSubstitute Documentation](https://nsubstitute.github.io/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Developer Guide](Developer-Guide) - Contributing guidelines
- [Architecture](Architecture) - System architecture

---

**Navigation**: [Home](Home) | [Developer Guide](Developer-Guide) | [Testing Guide](Testing-Guide) | [Architecture](Architecture)
