# Global Hotkey Customization

This document describes how to customize or disable the global hotkeys in the Idasen Desk application.

## Overview

The Idasen Desk application supports customizing global hotkeys through the `Settings.json` configuration file. This allows you to:

1. **Disable all global hotkeys** to prevent conflicts with other applications
2. **Customize key combinations** to use different keys and modifiers

## Finding Your Settings File

The location of your `Settings.json` file is shown in the application:

1. Open the Idasen Desk application
2. Go to **Settings** → **Advanced Settings**
3. Look for **Settings File** - this shows the full path to your configuration file

Typical location: `C:\Users\<YourUsername>\AppData\Roaming\Idasen.SystemTray\Settings.json`

## Configuration Options

### Option 1: Disable All Global Hotkeys

If you want to completely disable global hotkeys, add the following to your `Settings.json`:

```json
{
  "hotkeySettings": {
	"globalHotkeysEnabled": false
  }
}
```

After adding this configuration and restarting the application, no global hotkeys will be registered.

### Option 2: Customize Key Combinations

To change the key combinations, specify custom keys and modifiers for each position:

```json
{
  "hotkeySettings": {
	"globalHotkeysEnabled": true,
	"standingKey": "F1",
	"standingModifiers": "Control, Shift",
	"seatingKey": "F2",
	"seatingModifiers": "Control, Shift",
	"custom1Key": "F3",
	"custom1Modifiers": "Control, Shift",
	"custom2Key": "F4",
	"custom2Modifiers": "Control, Shift"
  }
}
```

This example configures:
- **Standing Position**: `Ctrl + Shift + F1`
- **Seating Position**: `Ctrl + Shift + F2`
- **Custom Position 1**: `Ctrl + Shift + F3`
- **Custom Position 2**: `Ctrl + Shift + F4`

## Available Keys

You can use any valid keyboard key string, including:

- **Function Keys**: `F1`, `F2`, `F3`, ... `F12`
- **Arrow Keys**: `Up`, `Down`, `Left`, `Right`
- **Letter Keys**: `A`, `B`, `C`, ... `Z`
- **Number Keys**: `D0`, `D1`, `D2`, ... `D9` (use D prefix for number row keys)
- **Special Keys**: `Space`, `Enter`, `Tab`, `PageUp`, `PageDown`, `Home`, `End`, `Insert`, `Delete`

For a complete list of available keys, refer to the [.NET Key enumeration documentation](https://learn.microsoft.com/en-us/dotnet/api/system.windows.input.key).

## Available Modifiers

Modifiers can be combined using commas and spaces:

- `Control`
- `Alt`
- `Shift`
- `Windows`

**Examples:**
- Single modifier: `"Control"`
- Two modifiers: `"Control, Shift"`
- Three modifiers: `"Control, Alt, Shift"`
- All modifiers: `"Control, Alt, Shift, Windows"`

## Complete Configuration Example

Here's a complete `Settings.json` file with custom hotkeys:

```json
{
  "deviceSettings": {
	"deviceName": "Desk",
	"deviceAddress": 0,
	"deviceMonitoringTimeout": 600,
	"deviceLocked": false,
	"notificationsEnabled": true
  },
  "heightSettings": {
	"standingHeightInCm": 120,
	"seatingHeightInCm": 65,
	"custom1HeightInCm": 100,
	"custom2HeightInCm": 80,
	"deskMinHeightInCm": 60,
	"deskMaxHeightInCm": 127,
	"lastKnownDeskHeight": 65,
	"standingIsVisibleInContextMenu": true,
	"seatingIsVisibleInContextMenu": true,
	"custom1IsVisibleInContextMenu": true,
	"custom2IsVisibleInContextMenu": true
  },
  "appearanceSettings": {
	"themeName": "theme_light"
  },
  "hotkeySettings": {
	"globalHotkeysEnabled": true,
	"standingKey": "F9",
	"standingModifiers": "Control, Alt",
	"seatingKey": "F10",
	"seatingModifiers": "Control, Alt",
	"custom1Key": "F11",
	"custom1Modifiers": "Control, Alt",
	"custom2Key": "F12",
	"custom2Modifiers": "Control, Alt"
  }
}
```

## Default Configuration

If the `hotkeySettings` section is missing from your `Settings.json`, the application will use these defaults:

- **Global Hotkeys Enabled**: `true`
- **Standing**: `Ctrl + Alt + Shift + Up`
- **Seating**: `Ctrl + Alt + Shift + Down`
- **Custom 1**: `Ctrl + Alt + Shift + Left`
- **Custom 2**: `Ctrl + Alt + Shift + Right`

## Important Notes

1. **Restart Required**: After editing `Settings.json`, you must restart the Idasen Desk application for changes to take effect.

2. **Backup Your Settings**: Before making changes, consider backing up your `Settings.json` file.

3. **JSON Syntax**: Ensure your JSON syntax is correct. Use a JSON validator if needed.

4. **Conflicting Hotkeys**: If your chosen hotkey is already registered by another application, you'll see a warning notification, and that specific hotkey won't work.

5. **Case Sensitivity**: Key names are case-insensitive (`"F1"` or `"f1"` both work), but it's recommended to use proper capitalization for readability.

## Troubleshooting

### Hotkeys Not Working

1. **Check the logs**: Find the log folder location in Settings → Advanced Settings → Log Folder
2. Look for messages about hotkey registration
3. Ensure no syntax errors in your `Settings.json`

### Invalid Key Configuration

If you specify an invalid key or modifier, the application will:
1. Log a warning message
2. Fall back to the default key combination for that position
3. Continue running normally

### Hotkey Conflicts

If another application has already registered your chosen hotkey combination, you'll receive a warning notification. Try:
1. Choosing a different key combination
2. Closing the conflicting application
3. Disabling global hotkeys in the conflicting application

## Examples

### Minimal Configuration - Disable Hotkeys Only

```json
{
  "hotkeySettings": {
	"globalHotkeysEnabled": false
  }
}
```

### Simple F-Key Configuration

```json
{
  "hotkeySettings": {
	"globalHotkeysEnabled": true,
	"standingKey": "F5",
	"seatingKey": "F6",
	"custom1Key": "F7",
	"custom2Key": "F8"
  }
}
```

Note: When you don't specify modifiers, they default to `"Control, Alt, Shift"`.

### Numpad Configuration

```json
{
  "hotkeySettings": {
	"globalHotkeysEnabled": true,
	"standingKey": "NumPad8",
	"standingModifiers": "Control",
	"seatingKey": "NumPad2",
	"seatingModifiers": "Control",
	"custom1Key": "NumPad4",
	"custom1Modifiers": "Control",
	"custom2Key": "NumPad6",
	"custom2Modifiers": "Control"
  }
}
```

## Support

If you encounter issues or need help with hotkey configuration:

1. Check the application logs
2. Visit the [GitHub Issues](https://github.com/tschroedter/idasen-desk/issues) page
3. Include your (sanitized) `Settings.json` configuration when reporting issues
