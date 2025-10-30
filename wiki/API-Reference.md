# API Reference

Technical API reference for the Idasen Desk Controller application.

> **Note**: This is a desktop application, not a library. This page documents the internal API for developers working on the codebase.

## Overview

This reference covers the main interfaces and classes used in the Idasen Desk Controller. For architectural overview, see [Architecture](Architecture).

## Core Interfaces

### IDeskService

Main interface for desk control operations.

```csharp
public interface IDeskService
{
    /// <summary>
    /// Connects to the Idasen desk via Bluetooth LE.
    /// </summary>
    /// <returns>True if connection successful, false otherwise.</returns>
    Task<bool> ConnectAsync();

    /// <summary>
    /// Disconnects from the currently connected desk.
    /// </summary>
    Task DisconnectAsync();

    /// <summary>
    /// Moves the desk to the specified height.
    /// </summary>
    /// <param name="targetHeight">Target height in centimeters.</param>
    /// <returns>True if movement successful, false otherwise.</returns>
    Task<bool> MoveToHeightAsync(int targetHeight);

    /// <summary>
    /// Stops the desk movement immediately.
    /// </summary>
    Task StopAsync();

    /// <summary>
    /// Gets the current connection status.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Gets the current desk height in centimeters.
    /// </summary>
    int? CurrentHeight { get; }

    /// <summary>
    /// Event raised when desk height changes.
    /// </summary>
    event EventHandler<int> HeightChanged;

    /// <summary>
    /// Event raised when connection status changes.
    /// </summary>
    event EventHandler<ConnectionStatus> ConnectionStatusChanged;
}
```

### ISettingsService

Interface for application settings management.

```csharp
public interface ISettingsService
{
    /// <summary>
    /// Loads settings from storage.
    /// </summary>
    /// <returns>Application settings.</returns>
    Settings LoadSettings();

    /// <summary>
    /// Saves settings to storage.
    /// </summary>
    /// <param name="settings">Settings to save.</param>
    void SaveSettings(Settings settings);

    /// <summary>
    /// Gets the current settings instance.
    /// </summary>
    Settings CurrentSettings { get; }

    /// <summary>
    /// Event raised when settings are changed.
    /// </summary>
    event EventHandler<Settings> SettingsChanged;

    /// <summary>
    /// Resets settings to default values.
    /// </summary>
    void ResetToDefaults();
}
```

### IHotkeyService

Interface for global hotkey management.

```csharp
public interface IHotkeyService
{
    /// <summary>
    /// Registers a global hotkey.
    /// </summary>
    /// <param name="name">Unique identifier for the hotkey.</param>
    /// <param name="keys">Key combination.</param>
    void RegisterHotkey(string name, KeyCombination keys);

    /// <summary>
    /// Unregisters a previously registered hotkey.
    /// </summary>
    /// <param name="name">Hotkey identifier.</param>
    void UnregisterHotkey(string name);

    /// <summary>
    /// Unregisters all hotkeys.
    /// </summary>
    void UnregisterAll();

    /// <summary>
    /// Event raised when a registered hotkey is pressed.
    /// </summary>
    event EventHandler<string> HotkeyPressed;
}
```

### ISystemTrayManager

Interface for system tray operations.

```csharp
public interface ISystemTrayManager
{
    /// <summary>
    /// Initializes the system tray icon.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Updates the tray icon to display the current height.
    /// </summary>
    /// <param name="height">Current height in centimeters.</param>
    void UpdateIcon(int height);

    /// <summary>
    /// Shows a balloon notification.
    /// </summary>
    /// <param name="title">Notification title.</param>
    /// <param name="message">Notification message.</param>
    /// <param name="icon">Icon type.</param>
    void ShowNotification(string title, string message, NotificationIcon icon);

    /// <summary>
    /// Updates the context menu items.
    /// </summary>
    void UpdateMenu();

    /// <summary>
    /// Disposes of system tray resources.
    /// </summary>
    void Dispose();
}
```

## Data Models

### Settings

Application settings data model.

```csharp
public class Settings
{
    /// <summary>
    /// Desk position configurations.
    /// </summary>
    public DeskPositions Positions { get; set; }

    /// <summary>
    /// Hotkey configurations.
    /// </summary>
    public HotkeySettings Hotkeys { get; set; }

    /// <summary>
    /// Appearance settings.
    /// </summary>
    public AppearanceSettings Appearance { get; set; }

    /// <summary>
    /// Desk connection settings.
    /// </summary>
    public DeskConnectionSettings Connection { get; set; }

    /// <summary>
    /// Application behavior settings.
    /// </summary>
    public ApplicationSettings Application { get; set; }
}
```

### DeskPositions

Desk position configuration.

```csharp
public class DeskPositions
{
    /// <summary>
    /// Standing position configuration.
    /// </summary>
    public Position Standing { get; set; }

    /// <summary>
    /// Seating position configuration.
    /// </summary>
    public Position Seating { get; set; }

    /// <summary>
    /// Custom position 1 configuration.
    /// </summary>
    public Position Custom1 { get; set; }

    /// <summary>
    /// Custom position 2 configuration.
    /// </summary>
    public Position Custom2 { get; set; }
}

public class Position
{
    /// <summary>
    /// Height in centimeters.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Whether this position is visible in the tray menu.
    /// </summary>
    public bool Visible { get; set; }
}
```

### ConnectionStatus

Desk connection status enumeration.

```csharp
public enum ConnectionStatus
{
    /// <summary>
    /// Not connected to desk.
    /// </summary>
    Disconnected,

    /// <summary>
    /// Attempting to connect.
    /// </summary>
    Connecting,

    /// <summary>
    /// Successfully connected.
    /// </summary>
    Connected,

    /// <summary>
    /// Connection failed or lost.
    /// </summary>
    Error
}
```

## ViewModels

### MainViewModel

Main application view model.

```csharp
public class MainViewModel : ViewModelBase
{
    /// <summary>
    /// Command to connect to desk.
    /// </summary>
    public ICommand ConnectCommand { get; }

    /// <summary>
    /// Command to disconnect from desk.
    /// </summary>
    public ICommand DisconnectCommand { get; }

    /// <summary>
    /// Command to move to standing position.
    /// </summary>
    public ICommand MoveToStandingCommand { get; }

    /// <summary>
    /// Command to move to seating position.
    /// </summary>
    public ICommand MoveToSeatingCommand { get; }

    /// <summary>
    /// Command to stop desk movement.
    /// </summary>
    public ICommand StopCommand { get; }

    /// <summary>
    /// Gets the current connection status.
    /// </summary>
    public ConnectionStatus ConnectionStatus { get; }

    /// <summary>
    /// Gets the current desk height.
    /// </summary>
    public int? CurrentHeight { get; }

    /// <summary>
    /// Gets whether the desk is currently connected.
    /// </summary>
    public bool IsConnected { get; }
}
```

### SettingsViewModel

Settings window view model.

```csharp
public class SettingsViewModel : ViewModelBase
{
    /// <summary>
    /// Command to save settings.
    /// </summary>
    public ICommand SaveCommand { get; }

    /// <summary>
    /// Command to reset settings to defaults.
    /// </summary>
    public ICommand ResetCommand { get; }

    /// <summary>
    /// Gets or sets the current settings.
    /// </summary>
    public Settings Settings { get; set; }

    /// <summary>
    /// Gets whether settings have unsaved changes.
    /// </summary>
    public bool HasChanges { get; }
}
```

## Bluetooth Communication

### GATT Services and Characteristics

The desk uses Bluetooth LE GATT protocol:

**Control Service UUID**: `99fa0001-338a-1024-8a49-009c0215f78a`
- Command Characteristic: `99fa0002-338a-1024-8a49-009c0215f78a` (Write)
- Reference Characteristic: `99fa0031-338a-1024-8a49-009c0215f78a` (Read)

**Height Service UUID**: `99fa0020-338a-1024-8a49-009c0215f78a`
- Height Characteristic: `99fa0021-338a-1024-8a49-009c0215f78a` (Read, Notify)

### Command Bytes

**Move Up**: `0x47 0x00`
**Move Down**: `0x46 0x00`
**Stop**: `0xFF 0x00`

### Height Data Format

Height is reported as a 16-bit little-endian integer in units of 0.1mm:
```csharp
// Convert to centimeters
int heightInCm = BitConverter.ToInt16(data, 0) / 100;
```

## Extension Methods

Useful extension methods available in the codebase:

```csharp
// Task extensions
public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout)

// Collection extensions
public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)

// String extensions
public static bool IsNullOrWhiteSpace(this string value)
```

## Error Handling

### Custom Exceptions

```csharp
/// <summary>
/// Exception thrown when Bluetooth operations fail.
/// </summary>
public class BluetoothException : Exception
{
    public BluetoothException(string message) : base(message) { }
    public BluetoothException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// Exception thrown when settings operations fail.
/// </summary>
public class SettingsException : Exception
{
    public SettingsException(string message) : base(message) { }
    public SettingsException(string message, Exception inner) : base(message, inner) { }
}
```

## Logging

The application uses `Microsoft.Extensions.Logging`:

```csharp
// Log levels
_logger.LogTrace("Detailed diagnostic information");
_logger.LogDebug("Debug information");
_logger.LogInformation("Informational message");
_logger.LogWarning("Warning message");
_logger.LogError(exception, "Error message");
_logger.LogCritical(exception, "Critical error");

// Structured logging
_logger.LogInformation("Connected to desk: {DeskName} at {Height}cm", 
    deskName, height);
```

## Dependency Injection

Services are registered in the IoC container:

```csharp
// Singleton services
builder.RegisterType<DeskService>().As<IDeskService>().SingleInstance();
builder.RegisterType<SettingsService>().As<ISettingsService>().SingleInstance();

// Per-dependency instances
builder.RegisterType<MainViewModel>().AsSelf();

// Registration with parameters
builder.Register(c => new Logger(logPath))
       .As<ILogger>()
       .SingleInstance();
```

## Usage Examples

### Connecting to Desk

```csharp
var deskService = container.Resolve<IDeskService>();

// Subscribe to events
deskService.HeightChanged += (s, height) => 
    Console.WriteLine($"Height: {height}cm");

deskService.ConnectionStatusChanged += (s, status) => 
    Console.WriteLine($"Status: {status}");

// Connect
bool connected = await deskService.ConnectAsync();
if (connected)
{
    Console.WriteLine("Connected successfully");
}
```

### Moving to Position

```csharp
// Move to 120cm
bool success = await deskService.MoveToHeightAsync(120);
if (success)
{
    Console.WriteLine("Movement completed");
}
else
{
    Console.WriteLine("Movement failed");
}
```

### Registering Hotkeys

```csharp
var hotkeyService = container.Resolve<IHotkeyService>();

// Subscribe to event
hotkeyService.HotkeyPressed += (s, name) =>
{
    Console.WriteLine($"Hotkey pressed: {name}");
    // Handle hotkey
};

// Register hotkeys
hotkeyService.RegisterHotkey("Standing", 
    new KeyCombination(Key.Up, ModifierKeys.Control | ModifierKeys.Shift));
```

### Managing Settings

```csharp
var settingsService = container.Resolve<ISettingsService>();

// Load settings
var settings = settingsService.LoadSettings();

// Modify settings
settings.Positions.Standing.Height = 120;

// Save settings
settingsService.SaveSettings(settings);

// Subscribe to changes
settingsService.SettingsChanged += (s, newSettings) =>
{
    Console.WriteLine("Settings changed");
};
```

## Platform-Specific Notes

### Windows Requirements

- Windows 10 version 1809 (build 17763) or later
- Bluetooth LE support required
- WPF (.NET 8.0 with Windows 10 target)

### Bluetooth LE APIs

Uses `Windows.Devices.Bluetooth` namespace:
- `BluetoothLEDevice` - Device representation
- `GattDeviceService` - GATT service access
- `GattCharacteristic` - Characteristic read/write/notify

## Related Documentation

- [Architecture](Architecture) - System architecture
- [Developer Guide](Developer-Guide) - Development guidelines
- [Testing Guide](Testing-Guide) - Testing approach
- [Build Instructions](Build-Instructions) - Building the project

---

**Navigation**: [Home](Home) | [Developer Guide](Developer-Guide) | [API Reference](API-Reference) | [Architecture](Architecture)
