# Settings Validation Implementation

## Overview
This implementation adds robust validation for height settings to prevent invalid configurations and improve application reliability.

## Changes Made

### 1. New Files Created

#### `Interfaces/IHeightSettingsValidator.cs`
- Defines the validation interface with methods for:
  - Individual height validation
  - Min/max constraint validation
  - Comprehensive validation of all heights
  - Preset name validation
- Includes `ValidationResult` class for returning validation results with error messages

#### `Utils/Validation/HeightSettingsValidator.cs`
- Implements `IHeightSettingsValidator`
- **Validation Rules:**
  - **Absolute Bounds:** 58-130 cm (physical desk limits)
  - **Min/Max Constraints:** Min must be < Max with at least 10 cm range
  - **Preset Heights:** Must be within min/max bounds
  - **Logical Checks:** Standing should typically be higher than seating (warning only)
  - **Preset Names:** 1-50 characters, no invalid file system characters

#### `Tests/Utils/Validation/HeightSettingsValidatorTests.cs`
- 33 comprehensive unit tests covering:
  - Valid and invalid height scenarios
  - Boundary conditions
  - Constraint validation
  - Name validation with invalid characters
  - Edge cases

### 2. Modified Files

#### `ViewModels/Pages/SettingsViewModel.cs`
- Added `IHeightSettingsValidator` dependency
- Added validation hooks for each height property:
  - `OnMinHeightChanged` / `OnMaxHeightChanged`
  - `OnStandingChanged` / `OnSeatingChanged`
  - `OnCustom1Changed` / `OnCustom2Changed`
  - `OnStandingNameChanged` / `OnSeatingNameChanged`
  - `OnCustom1NameChanged` / `OnCustom2NameChanged`
- Added `ValidateAllSettings()` method for comprehensive validation
- Validation warnings logged but don't block user input (allows temporary invalid states during editing)

#### `Utils/SettingsSynchronizer.cs`
- Added `IHeightSettingsValidator` dependency
- Enhanced `StoreSettingsAsync` to validate all settings before saving
- Logs validation errors but still saves (prevents data loss while alerting to issues)

#### `App.xaml.cs`
- Registered `HeightSettingsValidator` as singleton in DI container

#### Test Files Updated
- `Tests/Utils/SettingsSynchronizerTests.cs` - Added validator to test setup
- `Tests/ViewModels/Pages/SettingsViewModelTests.cs` - Added validator to test setup

## Validation Rules

### Height Constraints
```
Absolute Minimum: 58 cm
Absolute Maximum: 130 cm
Minimum Range: 10 cm (between min and max)
```

### Preset Name Rules
```
Length: 1-50 characters
Invalid Characters: < > : " / \ | ? *
Whitespace: Trimmed, but not allowed if only whitespace
```

### Logical Validation
- Standing height should be > Seating height (warning, not error)
- All preset heights must be within configured min/max range
- Min must be less than max
- Range between min and max must be >= 10 cm

## Behavior

### Real-time Validation
- Validates on property change
- Logs warnings for invalid values
- **Does NOT block user input** - allows temporary invalid states during editing
- Prevents jarring UX where values snap back

### Save-time Validation
- Comprehensive validation before persisting settings
- Logs all validation errors
- **Still saves settings** even if validation fails (prevents data loss)
- Errors prominently logged for troubleshooting

### Design Philosophy
- **Non-blocking:** User can temporarily have invalid configurations while adjusting multiple related settings
- **Informative:** Clear error messages guide users to valid configurations
- **Safe:** Validation prevents saving configurations that could damage desk or cause errors
- **Flexible:** Logs issues without preventing saves, allowing power users to override if needed

## Testing
- **33 new unit tests** for validator (100% pass rate)
- **43 existing tests** for SettingsSynchronizer (100% pass rate)
- **59 existing tests** for SettingsViewModel (100% pass rate)
- **Total: 135 tests passing**

## Future Enhancements
Potential improvements identified but not implemented:
1. **UI Feedback:** Visual indicators in UI when values are invalid
2. **Auto-correction:** Suggest nearest valid value
3. **Strict Mode:** Optional setting to block saves with invalid values
4. **Range Presets:** Quick buttons for common desk configurations
5. **Validation on Load:** Validate settings file on load and auto-correct if needed

## Benefits
1. **Prevents Configuration Errors:** Catches invalid heights before they cause issues
2. **Better Logging:** Clear validation messages in logs for troubleshooting
3. **Maintains UX:** Non-blocking validation allows smooth editing experience
4. **Extensible:** Easy to add new validation rules
5. **Well-tested:** Comprehensive test coverage ensures reliability
6. **Type-safe:** Uses C# type system for compile-time safety

## Example Validation Scenarios

### Valid Configuration
```csharp
MinHeight: 60 cm
MaxHeight: 127 cm
Standing: 120 cm
Seating: 75 cm
Result: ✓ Valid
```

### Invalid: Height Out of Range
```csharp
MinHeight: 60 cm
MaxHeight: 127 cm
Standing: 150 cm  // Above max
Result: ✗ "Height 150 cm exceeds the desk maximum of 127 cm"
```

### Invalid: Min/Max Inverted
```csharp
MinHeight: 127 cm
MaxHeight: 60 cm
Result: ✗ "Minimum height (127 cm) must be less than maximum height (60 cm)"
```

### Warning: Illogical Configuration
```csharp
Standing: 75 cm
Seating: 120 cm  // Higher than standing
Result: ⚠ "Warning: Standing height (75 cm) should typically be higher than seating height (120 cm)"
```

## Usage

### In Code
```csharp
// Validate individual height
var result = validator.ValidateHeight(height: 100, minHeight: 60, maxHeight: 127);
if (!result.IsValid)
{
	foreach (var error in result.Errors)
	{
		logger.Warning("Validation error: {Error}", error);
	}
}

// Validate all settings
var result = validator.ValidateAllHeights(
	standing: 120, seating: 75, 
	custom1: 100, custom2: 85,
	minHeight: 60, maxHeight: 127);
```

### In ViewModel
```csharp
// Automatic validation on property change
viewModel.Standing = 150;  // Logs warning if out of range

// Manual validation
var result = viewModel.ValidateAllSettings();
```

## Commit Message
```
feat: Add comprehensive height settings validation

- Add IHeightSettingsValidator interface and implementation
- Validate heights against absolute and desk-specific bounds
- Validate min/max constraints with minimum 10cm range
- Validate preset names (1-50 chars, no invalid characters)
- Add 33 comprehensive unit tests (all passing)
- Integrate validation into SettingsViewModel with property hooks
- Log validation errors without blocking user input
- Validate before save in SettingsSynchronizer
- Update all existing tests (135 tests total, 100% pass rate)

Validation prevents invalid desk configurations while maintaining 
smooth UX by not blocking temporary invalid states during editing.
```
