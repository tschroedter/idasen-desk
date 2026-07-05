# InMemoryLogger - Serilog Test Helper

## Overview

`InMemoryLogger` is a reusable test utility for capturing and asserting Serilog log output in unit tests. It provides a real `ILogger` instance that writes to an in-memory buffer, allowing you to verify logging behavior without using mocks.

## Why InMemoryLogger?

After updating NuGet packages, using NSubstitute with `ILogger` caused conflicts with Castle DynamicProxy when testing classes decorated with `[Intercept(typeof(LogAspect))]`. The solution is to use a real logger that captures output instead of mocking.

## Location

```
src/Idasen.BluetoothLE.Characteristics.Tests/Common/InMemoryLogger.cs
```

The class can be used across all test projects.

## Future options (ToDo)

- Use the Modern FakeLogger (Native .NET 8+)
- using Microsoft.Extensions.Logging.Testing;
- use Microsoft.Extensions.Logging.ILogger

## Basic Usage

### 1. Simple Log Verification

```csharp
[TestMethod]
public void Test_MethodLogsExpectedMessage()
{
    // Arrange
    using var InMemoryLogger = new InMemoryLogger();
    var sut = new YourClass(InMemoryLogger.Logger);

    // Act
    sut.DoSomething();

    // Assert
    InMemoryLogger.Contains("Found GattDeviceService with UUID")
              .Should()
              .BeTrue();
}
```

### 2. With Minimum Log Level

```csharp
[TestMethod]
public void Test_OnlyCaptureWarningsAndErrors()
{
    // Arrange - Only capture Warning and above
    using var InMemoryLogger = new InMemoryLogger(LogEventLevel.Warning);
    var sut = new YourClass(InMemoryLogger.Logger);

    // Act
    sut.DoSomething();

    // Assert
    InMemoryLogger.Contains("Error occurred")
              .Should()
              .BeTrue();

    // Debug messages won't be captured
    InMemoryLogger.Contains("Debug message")
              .Should()
              .BeFalse();
}
```

### 3. Count Log Occurrences

```csharp
[TestMethod]
public void Test_LogsMultipleItems()
{
    // Arrange
    using var InMemoryLogger = new InMemoryLogger();
    var sut = new YourClass(InMemoryLogger.Logger);

    // Act
    sut.ProcessItems();

    // Assert
    InMemoryLogger.CountLinesContaining("Processing item")
              .Should()
              .Be(5);
}
```

### 4. Use LINQ on Log Lines

```csharp
[TestMethod]
public void Test_NoErrorsLogged()
{
    // Arrange
    using var InMemoryLogger = new InMemoryLogger();
    var sut = new YourClass(InMemoryLogger.Logger);

    // Act
    sut.DoSomething();

    // Assert
    InMemoryLogger.AnyLine(line => line.Contains("Error"))
              .Should()
              .BeFalse("because no errors should occur");
}
```

### 5. Case-Sensitive Matching

```csharp
[TestMethod]
public void Test_ExactLogMessage()
{
    // Arrange
    using var InMemoryLogger = new InMemoryLogger();

    // Act
    InMemoryLogger.Logger.Information("Processing STARTED");

    // Assert
    InMemoryLogger.Contains("processing started")        // Case-insensitive
              .Should()
              .BeTrue();

    InMemoryLogger.ContainsExact("Processing STARTED")   // Case-sensitive
              .Should()
              .BeTrue();
}
```

### 6. Clear and Reuse

```csharp
[TestMethod]
public void Test_MultipleOperations()
{
    // Arrange
    using var InMemoryLogger = new InMemoryLogger();
    var sut = new YourClass(InMemoryLogger.Logger);

    // Act - First operation
    sut.OperationOne();
    InMemoryLogger.Contains("Operation One complete").Should().BeTrue();

    // Clear logs between operations
    InMemoryLogger.Clear();

    // Act - Second operation
    sut.OperationTwo();

    // Assert - Only second operation logs are present
    InMemoryLogger.Contains("Operation One").Should().BeFalse();
    InMemoryLogger.Contains("Operation Two complete").Should().BeTrue();
}
```

## API Reference

### Constructor

```csharp
public InMemoryLogger(LogEventLevel minimumLevel = LogEventLevel.Verbose)
```

- **minimumLevel**: The minimum log level to capture (defaults to Verbose)

### Properties

- **`Logger`**: The real Serilog `ILogger` instance to pass to your system under test
- **`Output`**: All captured log output as a single string
- **`Lines`**: All captured log lines as an array

### Methods

- **`Contains(string expectedText)`**: Case-insensitive search for text in log output
- **`ContainsExact(string expectedText)`**: Case-sensitive search for text in log output
- **`AnyLine(Func<string, bool> predicate)`**: Check if any log line matches a predicate
- **`CountLinesContaining(string text)`**: Count how many lines contain the specified text
- **`Clear()`**: Clear all captured log output
- **`Dispose()`**: Clean up resources (use with `using` statement)

## Migration from Logger.None

### Before (using Logger.None)

```csharp
[TestInitialize]
public void Initialize()
{
    _logger = Logger.None;  // Can't verify logs
    _sut = new DeskMover(_logger, ...);
}

[TestMethod]
public void Test_Something()
{
    _sut.DoSomething();
    // No way to verify logging behavior
}
```

### After (using InMemoryLogger)

```csharp
private InMemoryLogger _InMemoryLogger;

[TestInitialize]
public void Initialize()
{
    _InMemoryLogger = new InMemoryLogger();
    _sut = new DeskMover(_InMemoryLogger.Logger, ...);
}

[TestCleanup]
public void Cleanup()
{
    _InMemoryLogger?.Dispose();
}

[TestMethod]
public void Test_Something()
{
    _sut.DoSomething();

    // Now we can verify logging behavior
    _InMemoryLogger.Contains("Expected log message")
               .Should()
               .BeTrue();
}
```

## Examples

See `InMemoryLoggerExamples.cs` in the same directory for complete working examples.

## Notes

- Always use `using` or dispose the InMemoryLogger to clean up resources
- The logger captures formatted log messages (template resolved with values)
- Log format is: `[Level] Message - ExceptionMessage`
- The InMemoryLogger is thread-safe for writing (Serilog guarantees this)
