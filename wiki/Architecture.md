# Architecture

Technical architecture overview of the Idasen Desk Controller application.

## Overview

The Idasen Desk Controller is built using modern .NET technologies with a focus on maintainability, testability, and user experience. This document describes the technical architecture, design patterns, and key components.

## Technology Stack

### Core Technologies

- **.NET 8.0** - Modern, cross-platform framework
- **C# 12** - Latest language features
- **WPF** - Windows Presentation Foundation for UI
- **Target Framework**: `net8.0-windows10.0.19041`

### Key Libraries

- **Windows.Devices.Bluetooth** - Bluetooth LE communication
- **Autofac** - Dependency injection container
- **Hardcodet.NotifyIcon.Wpf** - System tray integration
- **Microsoft.Extensions.Logging** - Structured logging

### Testing Libraries

- **xUnit** - Unit testing framework
- **NSubstitute** - Mocking library
- **FluentAssertions** - Fluent assertion library

## Architectural Patterns

### MVVM (Model-View-ViewModel)

The application follows the MVVM pattern for UI components:

```
┌─────────────────────────────────────────────────┐
│                    View (XAML)                   │
│  ┌────────────────────────────────────────────┐ │
│  │  UserControls, Windows, Styles             │ │
│  └────────────────────────────────────────────┘ │
└──────────────────┬──────────────────────────────┘
                   │ Data Binding
                   ↓
┌─────────────────────────────────────────────────┐
│                  ViewModel                       │
│  ┌────────────────────────────────────────────┐ │
│  │  Presentation Logic, Commands, Properties  │ │
│  └────────────────────────────────────────────┘ │
└──────────────────┬──────────────────────────────┘
                   │ Uses Services
                   ↓
┌─────────────────────────────────────────────────┐
│                Model / Services                  │
│  ┌────────────────────────────────────────────┐ │
│  │  Business Logic, Data Access, Bluetooth    │ │
│  └────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────┘
```

**Benefits:**
- Clear separation of concerns
- Testable presentation logic
- UI-independent business logic
- Data binding for reactive UI

### Dependency Injection

All components are registered in an IoC container (Autofac):

```csharp
// Service Registration
builder.RegisterType<DeskService>().As<IDeskService>().SingleInstance();
builder.RegisterType<SettingsService>().As<ISettingsService>().SingleInstance();
builder.RegisterType<MainViewModel>().AsSelf().SingleInstance();

// Dependency Resolution
var mainViewModel = container.Resolve<MainViewModel>();
```

**Benefits:**
- Loose coupling between components
- Easy testing with mocked dependencies
- Centralized configuration
- Lifetime management

## Component Architecture

### High-Level Structure

```
┌──────────────────────────────────────────────────────────┐
│                   Application Layer                       │
│  ┌────────────┐  ┌────────────┐  ┌────────────┐        │
│  │   Views    │  │ ViewModels │  │  Commands  │        │
│  └────────────┘  └────────────┘  └────────────┘        │
└──────────────────────────────────────────────────────────┘
                        │
                        ↓
┌──────────────────────────────────────────────────────────┐
│                   Service Layer                           │
│  ┌────────────┐  ┌────────────┐  ┌────────────┐        │
│  │    Desk    │  │  Settings  │  │   Hotkey   │        │
│  │  Service   │  │  Service   │  │  Service   │        │
│  └────────────┘  └────────────┘  └────────────┘        │
└──────────────────────────────────────────────────────────┘
                        │
                        ↓
┌──────────────────────────────────────────────────────────┐
│                Infrastructure Layer                       │
│  ┌────────────┐  ┌────────────┐  ┌────────────┐        │
│  │ Bluetooth  │  │    File    │  │   System   │        │
│  │     LE     │  │  Storage   │  │    Tray    │        │
│  └────────────┘  └────────────┘  └────────────┘        │
└──────────────────────────────────────────────────────────┘
```

### Layer Descriptions

**Application Layer**
- WPF Views and ViewModels
- User interface components
- Command handlers
- Data binding

**Service Layer**
- Business logic
- Coordination between components
- State management
- Application workflow

**Infrastructure Layer**
- External system interaction
- Bluetooth communication
- File I/O
- System tray integration

## Key Components

### 1. Desk Service

**Responsibility:** Manages Bluetooth LE communication with the desk.

```csharp
public interface IDeskService
{
    Task<bool> ConnectAsync();
    Task DisconnectAsync();
    Task<bool> MoveToHeightAsync(int targetHeight);
    Task StopAsync();
    event EventHandler<int> HeightChanged;
    event EventHandler<ConnectionStatus> ConnectionStatusChanged;
}
```

**Key Operations:**
- Device discovery and connection
- Height position commands
- Real-time height monitoring
- Connection state management

### 2. Settings Service

**Responsibility:** Manages application settings and persistence.

```csharp
public interface ISettingsService
{
    Settings LoadSettings();
    void SaveSettings(Settings settings);
    event EventHandler<Settings> SettingsChanged;
}
```

**Key Operations:**
- Load/save settings to JSON
- Settings validation
- Change notifications
- Per-user storage

### 3. Hotkey Service

**Responsibility:** Manages global keyboard shortcuts.

```csharp
public interface IHotkeyService
{
    void RegisterHotkey(string name, KeyCombination keys);
    void UnregisterHotkey(string name);
    event EventHandler<string> HotkeyPressed;
}
```

**Key Operations:**
- Global hotkey registration
- Windows hotkey API integration
- Key combination handling
- Hotkey event dispatch

### 4. System Tray Manager

**Responsibility:** Manages system tray icon and context menu.

```csharp
public interface ISystemTrayManager
{
    void Initialize();
    void UpdateIcon(int height);
    void ShowNotification(string title, string message);
    void UpdateMenu();
}
```

**Key Operations:**
- Tray icon creation and updates
- Context menu management
- Balloon notifications
- User interaction handling

## Data Flow

### Connecting to Desk

```
User Action (Click "Connect")
        ↓
    ViewModel Command
        ↓
    Desk Service
        ↓
Bluetooth LE API (Windows)
        ↓
Physical Desk Connection
        ↓
Connection Event
        ↓
    ViewModel Updates
        ↓
UI Updates (Connected State)
```

### Moving to Position

```
User Action (Hotkey/Menu)
        ↓
Confirmation Dialog
        ↓
    ViewModel Command
        ↓
    Desk Service
        ↓
Calculate Stopping Point
        ↓
Send Move Command (BLE)
        ↓
Monitor Height Changes
        ↓
Stop at Target
        ↓
Height Events
        ↓
UI Updates (Real-time)
```

## Bluetooth LE Communication

### Protocol

The desk uses Bluetooth LE GATT (Generic Attribute Profile):

**Services:**
- Control Service: Commands (move, stop)
- Position Service: Height reporting
- Reference Service: Calibration data

**Characteristics:**
- Height (Read/Notify): Current height
- Command (Write): Movement commands
- Position (Write): Target height

### Communication Flow

```
┌──────────────┐         ┌──────────────┐         ┌──────────────┐
│ Application  │         │   Windows    │         │  Desk (BLE)  │
│              │         │  Bluetooth   │         │              │
└──────┬───────┘         └──────┬───────┘         └──────┬───────┘
       │                        │                        │
       │  Connect Request       │                        │
       │───────────────────────>│                        │
       │                        │   BLE Connect          │
       │                        │───────────────────────>│
       │                        │   Connected            │
       │                        │<───────────────────────│
       │  Connected Event       │                        │
       │<───────────────────────│                        │
       │                        │                        │
       │  Write Command         │                        │
       │───────────────────────>│                        │
       │                        │   GATT Write           │
       │                        │───────────────────────>│
       │                        │                        │
       │                        │   Height Notification  │
       │                        │<───────────────────────│
       │  Height Event          │                        │
       │<───────────────────────│                        │
```

## State Management

### Application States

```
┌─────────────────┐
│  Disconnected   │ ◀─────────────────┐
└────────┬────────┘                   │
         │                            │
         │ Connect                    │ Disconnect
         │                            │
         ↓                            │
┌─────────────────┐                   │
│   Connecting    │ ──Error──────────►│
└────────┬────────┘                   │
         │                            │
         │ Success                    │
         │                            │
         ↓                            │
┌─────────────────┐                   │
│    Connected    │ ───────────────────┘
└────────┬────────┘
         │
         │ Move Command
         │
         ↓
┌─────────────────┐
│     Moving      │
└────────┬────────┘
         │
         │ Reached/Stopped
         │
         ↓
┌─────────────────┐
│    Connected    │
└─────────────────┘
```

### Settings State

- Loaded on startup
- Cached in memory
- Saved on changes
- Per-user isolation

## Threading Model

### UI Thread

- WPF dispatcher thread
- Handles all UI updates
- Command execution
- Event handlers

### Background Tasks

- Bluetooth operations (async)
- File I/O (async)
- Height monitoring (notifications)

### Synchronization

```csharp
// Update UI from background thread
await Application.Current.Dispatcher.InvokeAsync(() =>
{
    CurrentHeight = newHeight;
});
```

## Error Handling

### Strategy

1. **Try-Catch at Boundaries**
   - Service method calls
   - External API calls
   - File operations

2. **Logging**
   - All errors logged with context
   - Structured logging
   - Sensitive data masked

3. **User Feedback**
   - Notifications for user-relevant errors
   - Graceful degradation
   - Recovery suggestions

### Example

```csharp
try
{
    await _deskService.ConnectAsync();
}
catch (BluetoothException ex)
{
    _logger.LogError(ex, "Failed to connect to desk");
    ShowNotification("Connection Failed", "Check Bluetooth settings");
}
```

## Performance Considerations

### Optimization Strategies

1. **Async/Await**
   - Non-blocking operations
   - Responsive UI
   - Efficient resource use

2. **Caching**
   - Settings cached in memory
   - Bluetooth device handles cached
   - Menu items cached

3. **Event Debouncing**
   - Height updates throttled
   - UI updates batched
   - Prevent excessive redraws

4. **Lazy Loading**
   - Services created on-demand
   - Resources loaded when needed

## Security

### Considerations

1. **Data Protection**
   - Settings in user profile
   - No passwords stored
   - Bluetooth addresses masked in logs

2. **Bluetooth Security**
   - Uses Windows pairing
   - No custom authentication
   - Relies on OS security

3. **Code Security**
   - CodeQL scanning
   - SonarCloud analysis
   - Dependency updates

## Testing Strategy

### Unit Tests

- ViewModel logic
- Service business logic
- Utility functions
- Mock external dependencies

### Integration Tests

- Service interaction
- Settings persistence
- State management

### Manual Testing

- Bluetooth communication
- UI interaction
- System tray behavior

See [Testing Guide](Testing-Guide) for details.

## Build and Deployment

### Build Process

1. Restore NuGet packages
2. Compile solution
3. Run tests
4. Publish self-contained executable

### CI/CD

- GitHub Actions workflow
- Automated testing
- Quality checks
- Release creation

See [Build Instructions](Build-Instructions) for details.

## Future Architecture Considerations

### Potential Improvements

1. **Plug-in System**
   - Support for different desk models
   - Custom position presets
   - Third-party integrations

2. **Cloud Sync**
   - Settings synchronization
   - Multiple devices
   - Backup and restore

3. **Enhanced Monitoring**
   - Usage statistics
   - Health metrics
   - Desk maintenance reminders

4. **Internationalization**
   - Multi-language support
   - Localized resources
   - Regional settings

## Related Documentation

- [Developer Guide](Developer-Guide) - Contributing guidelines
- [Build Instructions](Build-Instructions) - Building from source
- [Testing Guide](Testing-Guide) - Testing approach
- [Configuration](Configuration) - Application configuration

---

**Navigation**: [Home](Home) | [Developer Guide](Developer-Guide) | [Architecture](Architecture) | [Testing Guide](Testing-Guide)
