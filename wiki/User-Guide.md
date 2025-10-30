# User Guide

This comprehensive guide covers all features and functionality of the Idasen Desk Controller.

## Table of Contents

- [System Tray Interface](#system-tray-interface)
- [Context Menu](#context-menu)
- [Settings Window](#settings-window)
- [Controlling Your Desk](#controlling-your-desk)
- [Hotkeys](#hotkeys)
- [Advanced Features](#advanced-features)
- [Best Practices](#best-practices)

## System Tray Interface

The application runs quietly in your Windows system tray, providing quick access to all features.

### System Tray Icon States

The icon displays your desk's current height:

**Unknown Height**

![Unknown Height](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/V0.0.222/System_Tray_Icon_Unknow_Height.png)

Displayed when:
- Application just started
- Desk not yet connected
- Height hasn't been read

**Known Height**

![Known Height](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/V0.0.222/System_Tray_Icon_Know_Height.png)

Shows the actual desk height in centimeters, updated automatically when the desk moves.

### Icon Interactions

- **Left Click**: Show/Hide the Settings window
- **Right Click**: Open the context menu

## Context Menu

Right-click the system tray icon to access quick actions:

![Context Menu](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/V0.1.77/System_Tray_Context_Menu.png)

### Menu Items

- **Show/Hide Settings** - Toggle the settings window
- **Connect** - Connect to your desk (shown when disconnected)
- **Disconnect** - Disconnect from your desk (shown when connected)
- **Standing** - Move to standing position
- **Seating** - Move to seating position
- **Custom 1** - Move to custom position 1
- **Custom 2** - Move to custom position 2
- **Stop** - Immediately stop desk movement
- **Exit** - Close the application

**Note**: Menu items for positions can be shown/hidden in Settings → General tab.

## Settings Window

Access comprehensive settings by clicking "Show Settings" from the context menu or clicking the system tray icon.

### General Settings

![General Settings](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/V0.1.77/Settings_General.png)

#### Desk Positions

Configure up to four preset positions:

**Standing Position**
- Your preferred standing height (typically 100-130 cm)
- Default hotkey: `Ctrl + Shift + Alt + ↑`

**Seating Position**
- Your preferred sitting height (typically 60-80 cm)
- Default hotkey: `Ctrl + Shift + Alt + ↓`

**Custom 1 Position**
- Additional custom position for specific tasks
- Default hotkey: `Ctrl + Shift + Alt + ←`

**Custom 2 Position**
- Second additional custom position
- Default hotkey: `Ctrl + Shift + Alt + →`

**For Each Position:**
- **Height Value**: Set in centimeters
- **Tray Checkbox**: Show/hide in context menu

**Tips for Setting Heights:**
- Move desk manually to desired height
- Note the height from desk display
- Enter value in settings
- Or use arrow keys during move confirmation to fine-tune

#### Tray Menu Options

- **Stop Command**: Toggle visibility of Stop command in context menu

### Hot Keys Settings

![Hotkey Settings](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/V0.1.77/Settins_Hotkeys.png)

Global hotkeys work in any application, even when the settings window is closed.

**Default Hotkeys:**
- Standing: `Ctrl + Shift + Alt + ↑`
- Seating: `Ctrl + Shift + Alt + ↓`
- Custom 1: `Ctrl + Shift + Alt + ←`
- Custom 2: `Ctrl + Shift + Alt + →`

**To Change a Hotkey:**
1. Click on the hotkey field
2. Press your desired key combination
3. Settings are saved automatically

**Hotkey Tips:**
- Use unique combinations to avoid conflicts
- Include at least one modifier (Ctrl, Shift, Alt)
- Test in different applications to ensure it works globally
- Avoid combinations used by other applications

### Appearance Settings

![Appearance Settings](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/V0.0.222/Settins_Appearance.png)

#### Theme Selection

Choose from multiple color themes to match your desktop environment:
- Light themes for bright environments
- Dark themes for low-light conditions
- High-contrast themes for accessibility

Changes apply immediately without restart.

### Advanced Settings

![Advanced Settings](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/V0.0.222/Settins_Advanced.png)

#### Log Folder
- Displays location of application log files
- Useful for troubleshooting
- Click to open folder in File Explorer

#### Settings File
- Shows where user settings are stored
- Each Windows user has separate settings
- Click to open folder in File Explorer

#### Desk Name
- Specify custom desk name if changed from default
- Default: App searches for devices starting with "Desk"
- Useful if you've renamed your desk

#### Desk Address
- Optional Bluetooth MAC address (as unsigned long)
- Enables faster connection
- Priority: Desk Name first, then Desk Address
- Find address in Windows Bluetooth settings or logs

#### Parental Lock
- **Enabled**: Locks physical desk controller
- When locked, pressing up/down buttons stops movement immediately
- Prevents unauthorized desk adjustments
- Useful in shared spaces or homes with children

#### Units Till Stop
- Fine-tunes stopping distance calculation
- Adjust if desk overshoots or undershoots target heights
- Higher value = earlier stop (before target)
- Lower value = later stop (may overshoot)
- Default value works for most users

#### Reset Settings
- Restores all settings to defaults
- **Warning**: This action cannot be undone
- You'll need to reconfigure positions and preferences

## Controlling Your Desk

### Methods of Control

There are three ways to control your desk:

1. **System Tray Context Menu** (right-click icon)
2. **Settings Window Menu** (left sidebar)
3. **Global Hotkeys** (keyboard shortcuts)

### Move Action Flow

When you trigger a move command:

1. **Confirmation Dialog Appears**
   - Shows target height
   - Displays current height
   - Provides fine-tuning controls

2. **Fine-Tune Position (Optional)**
   - Press `↑` to increase target height
   - Press `↓` to decrease target height
   - Press `←` or `→` to cancel
   - Each press adjusts by smallest increment

3. **Confirm or Cancel**
   - Click **"Move"** or press `Enter` to proceed
   - Click **"Cancel"** or press `Esc` to abort
   - Press `←` or `→` to cancel

4. **Desk Moves**
   - Desk automatically moves to target height
   - Icon updates in real-time
   - Notification appears when complete

5. **Position Saved**
   - If you fine-tuned the height, new value is automatically saved
   - No need to manually update settings

### Stop Command

To stop desk movement at any time:
- Right-click icon → **Stop**
- Press your Stop hotkey (if configured)
- Press physical buttons on desk controller

## Hotkeys

### Using Hotkeys

Global hotkeys work anywhere in Windows:
- No need to focus the application
- Works even when settings window is hidden
- Consistent experience across all applications

### Hotkey Best Practices

1. **Avoid Conflicts**
   - Check that chosen combinations don't conflict with other apps
   - Test in your most-used applications

2. **Memorability**
   - Use logical combinations (e.g., ↑ for Standing, ↓ for Seating)
   - Keep combinations similar for easy recall

3. **Accessibility**
   - Ensure combinations are easy to press
   - Consider ergonomics for frequent use

## Advanced Features

### Desktop Notifications

The application provides informative notifications for:

![Notifications](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/V0.0.222/Notifications.png)

- 🔄 **Connecting**: "Trying to connect to desk..."
- ✅ **Connected**: "Successfully connected"
- ❌ **Failed**: "Failed to connect"
- 📏 **Movement Complete**: "Desk reached target height"
- ⚠️ **Errors**: Connection issues, timeouts

### Auto-Connect

The application automatically:
- Connects to your desk on startup
- Reconnects if connection is lost
- Remembers your last connected desk
- Handles connection failures gracefully

### Per-User Settings

Settings are stored per Windows user:
- Each user can have different positions
- Custom hotkeys per user
- Individual preferences
- Shared desk, personal configuration

### Position Visibility Control

Customize which positions appear in the context menu:
- Useful if you only use 2-3 positions
- Cleaner, more focused menu
- Positions remain accessible via hotkeys even when hidden

## Best Practices

### Ergonomics

1. **Standing Position**
   - Elbows at 90° when typing
   - Monitor at eye level
   - Feet flat on floor

2. **Sitting Position**
   - Feet flat on floor or footrest
   - Knees at 90°
   - Lower back supported

3. **Alternating**
   - Switch positions every 30-60 minutes
   - Use hotkeys for easy transitions
   - Set reminders if needed

### Desk Maintenance

- Keep desk surface clear during movement
- Ensure cables have adequate slack
- Test emergency stop function
- Verify Bluetooth connection regularly

### Application Usage

1. **Start with Defaults**
   - Try default positions first
   - Adjust after a few days of use
   - Fine-tune as you discover preferences

2. **Use Hotkeys**
   - Faster than menu navigation
   - More ergonomic
   - Builds muscle memory

3. **Monitor Logs**
   - Check logs if issues occur
   - Logs help diagnose connection problems
   - Location shown in Advanced Settings

### Troubleshooting Tips

If something isn't working:
1. Check Bluetooth connection in Windows
2. Verify desk is paired
3. Restart application
4. Check log files
5. See **[Troubleshooting](Troubleshooting)** page

## Keyboard Shortcuts Reference

| Action | Default Shortcut |
|--------|------------------|
| Standing Position | `Ctrl + Shift + Alt + ↑` |
| Seating Position | `Ctrl + Shift + Alt + ↓` |
| Custom 1 Position | `Ctrl + Shift + Alt + ←` |
| Custom 2 Position | `Ctrl + Shift + Alt + →` |
| Show/Hide Settings | Click tray icon |
| Open Context Menu | Right-click tray icon |

---

**Navigation**: [Home](Home) | [Getting Started](Getting-Started) | [User Guide](User-Guide) | [Configuration](Configuration) | [Troubleshooting](Troubleshooting)
