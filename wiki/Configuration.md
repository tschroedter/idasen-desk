# Configuration

Advanced configuration options for the Idasen Desk Controller.

## Configuration File Location

Settings are stored in JSON format at:
```
%LOCALAPPDATA%\IdasenSystemTray\settings.json
```

Replace `%LOCALAPPDATA%` with your actual path:
```
C:\Users\[YourUsername]\AppData\Local\IdasenSystemTray\settings.json
```

**Note:** Settings are per-user. Each Windows user has separate configuration.

## Accessing Configuration

### Via Application UI

Most settings can be configured through the Settings window:
1. Right-click system tray icon
2. Select "Show Settings"
3. Navigate through tabs

### Via File Editor

For advanced users, you can edit the JSON file directly:
1. Exit the application
2. Open `settings.json` in a text editor
3. Make changes carefully
4. Save and restart application

**Warning:** Invalid JSON will cause settings to reset to defaults.

## Configuration Categories

### General Settings

These settings control desk positions and menu visibility.

#### Desk Positions

```json
{
  "Positions": {
    "Standing": {
      "Height": 120,
      "Visible": true
    },
    "Seating": {
      "Height": 75,
      "Visible": true
    },
    "Custom1": {
      "Height": 100,
      "Visible": true
    },
    "Custom2": {
      "Height": 90,
      "Visible": false
    }
  }
}
```

**Parameters:**
- `Height` - Desk height in centimeters (integer)
- `Visible` - Show in system tray context menu (boolean)

**Valid Height Range:** Typically 60-130 cm (depends on your desk model)

#### Menu Options

```json
{
  "MenuOptions": {
    "ShowStopCommand": true
  }
}
```

**Parameters:**
- `ShowStopCommand` - Display Stop command in context menu (boolean)

### Hotkey Settings

Configure global keyboard shortcuts for desk positions.

```json
{
  "Hotkeys": {
    "Standing": "Ctrl+Shift+Alt+Up",
    "Seating": "Ctrl+Shift+Alt+Down",
    "Custom1": "Ctrl+Shift+Alt+Left",
    "Custom2": "Ctrl+Shift+Alt+Right"
  }
}
```

**Format:** Modifiers + Key
- Modifiers: `Ctrl`, `Shift`, `Alt`, `Win`
- Keys: Standard key names (e.g., `Up`, `Down`, `A`, `F1`)
- Separator: `+`

**Examples:**
```json
"Standing": "Ctrl+Alt+S"
"Seating": "Ctrl+Alt+D"
"Custom1": "Win+Shift+1"
```

**Best Practices:**
- Always include at least one modifier
- Avoid conflicts with system hotkeys
- Use logical key combinations

### Appearance Settings

Control the visual theme of the application.

```json
{
  "Appearance": {
    "Theme": "Dark"
  }
}
```

**Valid Themes:**
- `Light` - Light color scheme
- `Dark` - Dark color scheme
- `HighContrast` - High contrast for accessibility
- (Additional themes may be available in your version)

### Advanced Settings

#### Desk Identification

```json
{
  "DeskConnection": {
    "DeskName": "Desk",
    "DeskAddress": null,
    "SearchByName": true
  }
}
```

**Parameters:**

**DeskName** (string)
- Name of your desk as it appears in Bluetooth settings
- Default: `"Desk"`
- Useful if you renamed your desk
- Application searches for devices starting with this name

**DeskAddress** (unsigned long, nullable)
- Bluetooth MAC address in unsigned long format
- Default: `null` (not used)
- When set, enables faster direct connection
- Format: 123456789012345 (example)

**SearchByName** (boolean)
- `true` - Search by name first, then address
- `false` - Use address only (if set)
- Default: `true`

##### Finding Your Desk Address

**Method 1: From Logs**
1. Connect to desk via application
2. Check log file: Settings → Advanced → Log Folder
3. Look for entry: "Connected to device with address: [number]"

**Method 2: From Windows Bluetooth Settings**
1. Open Settings → Bluetooth & devices
2. Click on your desk
3. Look for MAC address (e.g., `12:34:56:78:90:AB`)
4. Convert to decimal (use online converter)

#### Desk Behavior

```json
{
  "DeskBehavior": {
    "ParentalLock": false,
    "UnitsTillStop": 100,
    "AutoReconnect": true
  }
}
```

**Parameters:**

**ParentalLock** (boolean)
- `true` - Physical desk buttons stop movement immediately
- `false` - Physical buttons work normally
- Default: `false`
- Use case: Prevent unauthorized adjustments

**UnitsTillStop** (integer)
- Fine-tunes stopping distance calculation
- Higher value = stops earlier
- Lower value = stops later
- Default: `100` (optimal for most desks)
- Range: 0-500

**AutoReconnect** (boolean)
- `true` - Automatically reconnect if connection lost
- `false` - Manual reconnection required
- Default: `true`

#### Connection Settings

```json
{
  "Connection": {
    "TimeoutSeconds": 30,
    "RetryAttempts": 3,
    "RetryDelayMs": 1000
  }
}
```

**Parameters:**

**TimeoutSeconds** (integer)
- Connection timeout in seconds
- Default: `30`
- Range: 10-120

**RetryAttempts** (integer)
- Number of connection retry attempts
- Default: `3`
- Range: 1-10

**RetryDelayMs** (integer)
- Delay between retries in milliseconds
- Default: `1000` (1 second)
- Range: 500-5000

### Application Settings

```json
{
  "Application": {
    "StartMinimized": true,
    "ShowNotifications": true,
    "LogLevel": "Information"
  }
}
```

**Parameters:**

**StartMinimized** (boolean)
- `true` - Start in system tray only
- `false` - Show settings window on startup
- Default: `true`

**ShowNotifications** (boolean)
- `true` - Show desktop notifications
- `false` - Suppress notifications
- Default: `true`

**LogLevel** (string)
- Logging verbosity level
- Valid values: `"Trace"`, `"Debug"`, `"Information"`, `"Warning"`, `"Error"`, `"Critical"`
- Default: `"Information"`
- Use `"Debug"` for troubleshooting

## Example Configuration File

Complete example with common settings:

```json
{
  "Positions": {
    "Standing": {
      "Height": 120,
      "Visible": true
    },
    "Seating": {
      "Height": 75,
      "Visible": true
    },
    "Custom1": {
      "Height": 100,
      "Visible": true
    },
    "Custom2": {
      "Height": 90,
      "Visible": false
    }
  },
  "Hotkeys": {
    "Standing": "Ctrl+Shift+Alt+Up",
    "Seating": "Ctrl+Shift+Alt+Down",
    "Custom1": "Ctrl+Shift+Alt+Left",
    "Custom2": "Ctrl+Shift+Alt+Right"
  },
  "Appearance": {
    "Theme": "Dark"
  },
  "DeskConnection": {
    "DeskName": "Desk",
    "DeskAddress": null,
    "SearchByName": true
  },
  "DeskBehavior": {
    "ParentalLock": false,
    "UnitsTillStop": 100,
    "AutoReconnect": true
  },
  "Connection": {
    "TimeoutSeconds": 30,
    "RetryAttempts": 3,
    "RetryDelayMs": 1000
  },
  "Application": {
    "StartMinimized": true,
    "ShowNotifications": true,
    "LogLevel": "Information"
  },
  "MenuOptions": {
    "ShowStopCommand": true
  }
}
```

## Configuration Best Practices

### Backup Your Settings

Before manual editing:
```powershell
# PowerShell
Copy-Item "$env:LOCALAPPDATA\IdasenSystemTray\settings.json" `
         "$env:LOCALAPPDATA\IdasenSystemTray\settings.json.backup"
```

### Test Changes Incrementally

1. Make one change at a time
2. Restart application
3. Verify change works
4. Repeat for next change

### Reset to Defaults

If configuration is corrupted:

**Method 1: Via Application**
1. Open Settings → Advanced
2. Click "Reset Settings"
3. Confirm action

**Method 2: Manual**
1. Exit application
2. Delete `settings.json`
3. Restart application
4. Defaults are recreated

### Share Configuration

To use same settings on multiple computers:
1. Export settings from first computer
2. Copy `settings.json` to new computer
3. Place in correct location on new computer
4. Adjust if desk names/addresses differ

## Environment Variables

Some paths support environment variables:

**Supported Variables:**
- `%LOCALAPPDATA%` - User's local app data folder
- `%APPDATA%` - User's roaming app data folder
- `%USERPROFILE%` - User's profile folder
- `%TEMP%` - Temporary folder

**Example:**
```json
{
  "LogPath": "%LOCALAPPDATA%\\IdasenSystemTray\\Logs"
}
```

## Advanced Configuration Scenarios

### Multiple Users, One Desk

Each user should configure:
1. Their own preferred heights
2. Their own hotkeys
3. Settings are automatically separate

### Single User, Multiple Desks

Configure different desk names or addresses:
1. Change `DeskName` or `DeskAddress`
2. Restart application to connect to different desk
3. Consider keeping multiple config files and swapping

### Shared Workspace

When sharing a desk:
1. Use Parental Lock to prevent unwanted changes
2. Consider hiding unused positions
3. Disable auto-connect if multiple people use desk

### High-Latency Connection

If Bluetooth is slow:
1. Increase `TimeoutSeconds`
2. Increase `RetryAttempts`
3. Increase `RetryDelayMs`

## Troubleshooting Configuration Issues

### Settings Not Loading

1. Check JSON syntax (use JSON validator)
2. Verify file permissions (should be readable/writable)
3. Check for BOM or encoding issues (use UTF-8)
4. Review log files for errors

### Settings Reset on Startup

Causes:
- Invalid JSON syntax
- Corrupted file
- Permission issues
- Disk errors

Solution:
1. Restore from backup
2. Or reset to defaults and reconfigure

### Hotkeys Not Working After Change

1. Ensure no conflicts with other applications
2. Restart application
3. Verify syntax in configuration file
4. Check Windows hotkey registration

## Security Considerations

### Sensitive Data

The configuration file may contain:
- Desk names (potentially identifiable)
- Bluetooth addresses

**Protection:**
- File is in user profile (user-accessible only)
- No passwords or tokens stored
- No remote access to settings

### File Permissions

Default permissions:
- Read/Write for current user
- No access for other users
- No special admin rights required

## Related Resources

- [User Guide](User-Guide) - Using settings UI
- [Troubleshooting](Troubleshooting) - Fixing configuration issues
- [FAQ](FAQ) - Common configuration questions

---

**Navigation**: [Home](Home) | [User Guide](User-Guide) | [Configuration](Configuration) | [Advanced Topics](Advanced-Topics)
