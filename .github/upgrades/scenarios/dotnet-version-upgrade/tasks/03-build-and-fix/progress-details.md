# Task 03-build-and-fix: Build solution and resolve any issues

## What Changed

Fixed a nullable reference warning (CS8604) that became an error in .NET 10 due to stricter nullable reference type handling in framework event handlers.

## Issues Fixed

### CS8604: Possible null reference argument
- **Location**: `src/Idasen.SystemTray.Win11/Utils/PowerEventsWrapper.cs`, line 37
- **Issue**: PowerModeChangedEventHandler.Invoke requires non-null sender parameter, but SystemEvents.PowerModeChanged provides nullable sender
- **Root cause**: .NET 10 has stricter null-safety annotations in framework event handler delegates
- **Fix**: Changed `PowerModeChanged?.Invoke(sender, e)` to `PowerModeChanged?.Invoke(sender ?? this, e)` to provide non-null fallback

## Files Modified

- `src/Idasen.SystemTray.Win11/Utils/PowerEventsWrapper.cs` - Added null-coalescing operator to handle nullable sender parameter

## Validation Results

### Debug Build
- **Command**: Visual Studio build system
- **Result**: ✅ Build successful - 0 errors, 0 warnings
- **Output**: `Idasen.SystemTray.Win11.dll`, `Idasen.SystemTray.Win11.Tests.dll`

### Release Build
- **Command**: `dotnet build -c Release --no-restore`
- **Result**: ✅ Build successful - 0 errors, 0 warnings
- **Build time**: 3.1s
- **Output**: Both projects built successfully

### Solution Health
- ✅ All projects compile cleanly
- ✅ No warnings in either configuration
- ✅ TreatWarningsAsErrors remains enabled
- ✅ Nullable reference types working correctly

## Next Steps

Proceed to task 04-run-tests to verify that the code fix doesn't introduce behavioral regressions.
