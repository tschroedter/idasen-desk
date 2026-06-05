# Cancellation Token Management Improvements

## Overview
Centralized timeout management for cancellation tokens and other timing operations to improve testability and maintainability.

## Problem
Multiple hardcoded timeout values were scattered across the codebase:
- 60-second timeout in `UIDeskManager.Initialize()` and auto-connect path
- 10-second timer in `StatusBarInfoViewModelBase`
- 300ms throttle in `SettingsViewModel`

These magic numbers made testing difficult and reduced maintainability.

## Solution
Created `Constants.Timeouts` nested class to centralize all timeout configuration:

```csharp
public static class Timeouts
{
	/// <summary>
	///     Timeout for UI Desk Manager initialization and settings operations.
	///     Allows sufficient time for Bluetooth connection and settings load.
	/// </summary>
	public const int InitializationSeconds = 60;

	/// <summary>
	///     Delay in seconds before clearing status bar info message.
	///     Gives user time to read notification messages.
	/// </summary>
	public const int StatusBarInfoClearDelaySeconds = 10;

	/// <summary>
	///     Throttle delay in milliseconds for auto-save settings.
	///     Prevents excessive saves during rapid property changes.
	/// </summary>
	public const int SettingsAutoSaveThrottleMilliseconds = 300;
}
```

## Changes Made

### 1. UIDeskManager.cs
**Before:**
```csharp
_tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
```

**After:**
```csharp
_tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.Timeouts.InitializationSeconds));
```

Updated in two locations:
- Initialize() method (line 233)
- Auto-connect path (line 340)

### 2. StatusBarInfoViewModelBase.cs
**Before:**
```csharp
_timer = timerFactory(
	OnElapsed,
	null,
	TimeSpan.FromSeconds(10),
	Timeout.InfiniteTimeSpan);
```

**After:**
```csharp
_timer = timerFactory(
	OnElapsed,
	null,
	TimeSpan.FromSeconds(Constants.Timeouts.StatusBarInfoClearDelaySeconds),
	Timeout.InfiniteTimeSpan);
```

### 3. SettingsViewModel.cs
**Before:**
```csharp
.Throttle(TimeSpan.FromMilliseconds(300), _scheduler)
```

**After:**
```csharp
.Throttle(TimeSpan.FromMilliseconds(Constants.Timeouts.SettingsAutoSaveThrottleMilliseconds), _scheduler)
```

## Benefits

### 1. Easier Testing
- Timeout values can be referenced in tests
- Future enhancement: could make values configurable for testing
- Clear documentation of expected timing behavior

### 2. Improved Maintainability
- Single source of truth for timeout configuration
- Changes to timeouts require only one location update
- Clear intent through named constants and XML documentation

### 3. Better Code Readability
- Descriptive constant names explain purpose
- XML documentation provides context
- No magic numbers scattered in code

### 4. Configuration Flexibility
- Centralized location makes future configuration easier
- Could be extended to read from settings if needed
- Easier to adjust timeouts based on performance requirements

## Testing
All 351 tests pass with the new centralized timeout values, confirming:
- No behavioral changes
- Existing timeout durations preserved
- All dependent code updated correctly

## Usage Example
```csharp
// Creating a cancellation token with initialization timeout
var cts = new CancellationTokenSource(
	TimeSpan.FromSeconds(Constants.Timeouts.InitializationSeconds));

// Using in timer configuration
timer.Change(
	TimeSpan.FromSeconds(Constants.Timeouts.StatusBarInfoClearDelaySeconds),
	Timeout.InfiniteTimeSpan);

// Using in reactive throttle
observable.Throttle(
	TimeSpan.FromMilliseconds(Constants.Timeouts.SettingsAutoSaveThrottleMilliseconds),
	scheduler);
```

## Related Patterns
This follows the existing pattern in the codebase:
- `Constants.HeightChangeThrottleSeconds` (already existed)
- Other constants in `Constants.cs` for shared values

## Future Enhancements
Potential improvements:
1. Make timeouts configurable via settings file
2. Add timeout configuration for unit tests (shorter values)
3. Centralize other timing-related constants
4. Add telemetry for timeout-related cancellations

## Commit
```
a8193c2 - Centralize cancellation token timeout management
```

## Files Modified
- `Idasen.SystemTray.Win11/Utils/Constants.cs` - Added Timeouts nested class
- `Idasen.SystemTray.Win11/Utils/UIDeskManager.cs` - Updated two timeout usages
- `Idasen.SystemTray.Win11/ViewModels/Pages/StatusBarInfoViewModelBase.cs` - Updated timer delays
- `Idasen.SystemTray.Win11/ViewModels/Pages/SettingsViewModel.cs` - Updated throttle duration
