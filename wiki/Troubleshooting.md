# Troubleshooting

This guide helps you resolve common issues with the Idasen Desk Controller.

## Table of Contents

- [Connection Issues](#connection-issues)
- [Pairing Problems](#pairing-problems)
- [Movement Issues](#movement-issues)
- [Application Issues](#application-issues)
- [Performance Issues](#performance-issues)
- [Log Files](#log-files)
- [Getting More Help](#getting-more-help)

## Connection Issues

### Application Can't Find Desk

**Symptoms:**
- "Failed to connect" notification
- Application shows "Disconnected"
- Desk not responding to commands

**Solutions:**

1. **Verify Bluetooth Pairing**
   ```
   Settings → Bluetooth & devices
   ```
   - Ensure desk appears as "Desk" or your custom name
   - Status should be "Paired" or "Connected"
   - If not paired, see [Pairing Problems](#pairing-problems)

2. **Check Desk Is Not In Use**
   - Close Ikea mobile app if running
   - Disconnect other devices from desk
   - Only one connection at a time is supported

3. **Restart Bluetooth Service**
   ```powershell
   # Run in PowerShell as Administrator
   Restart-Service bthserv
   ```

4. **Restart Application**
   - Right-click tray icon → Exit
   - Wait 5 seconds
   - Start application again

5. **Restart Computer**
   - Sometimes Windows Bluetooth needs a full restart
   - Most reliable solution for persistent issues

### Connection Drops Frequently

**Symptoms:**
- Desk disconnects randomly
- Need to reconnect often
- Unstable connection

**Solutions:**

1. **Check Bluetooth Signal**
   - Ensure desk is within 10 meters of computer
   - Remove obstacles between desk and computer
   - Metal objects can interfere with Bluetooth

2. **Update Bluetooth Drivers**
   - Open Device Manager
   - Expand "Bluetooth" section
   - Right-click adapter → Update driver
   - Restart computer after update

3. **Check Power Management**
   - Open Device Manager
   - Bluetooth adapter → Properties
   - Power Management tab
   - Uncheck "Allow computer to turn off this device"

4. **Check for Interference**
   - WiFi routers nearby can cause interference
   - Other Bluetooth devices may conflict
   - Microwave ovens can interfere with 2.4 GHz
   - Try moving desk or computer

### Slow Connection Times

**Symptoms:**
- Takes long time to connect
- Multiple retry attempts
- Connection eventually succeeds

**Solutions:**

1. **Configure Desk Address**
   - Open Settings → Advanced
   - Find desk MAC address in Windows Bluetooth settings
   - Convert to unsigned long format
   - Enter in Desk Address field
   - This enables faster direct connection

2. **Ensure Single Device Name**
   - Check only one device named "Desk" is paired
   - Remove old/duplicate desk pairings
   - Rename desk to unique name if needed

## Pairing Problems

### Windows 11 Can't Find Desk

**Symptoms:**
- Desk not appearing in Bluetooth device list
- Pairing mode active but desk invisible

**Solutions:**

#### Solution 1: Enable Advanced Discovery

1. Open **Settings** → **Bluetooth & devices**
2. Enable **"Show all Bluetooth devices"**
3. Put desk in pairing mode
4. Desk should now appear

Reference: [Reddit discussion](https://www.reddit.com/r/cricut/comments/14h9sz8/windows_11_bluetooth_issues_fixed)

#### Solution 2: Use Legacy Devices Panel

1. **Settings** → **Bluetooth & devices** → **Devices**
2. Scroll down → **"More devices and printers settings"**
3. Right-click empty space → **"Add a device"**
4. Put desk in pairing mode
5. Select desk from list

### Desk Won't Enter Pairing Mode

**Symptoms:**
- LED not blinking
- Desk doesn't respond to pairing button

**Solutions:**

1. **Check Button Press**
   - Hold pairing button for 3-5 seconds
   - Button location varies by desk model
   - LED should start blinking

2. **Reset Desk Bluetooth**
   - Hold pairing button for 10+ seconds
   - This resets Bluetooth module
   - Wait 10 seconds, then retry

3. **Check Power**
   - Ensure desk is plugged in
   - Check power outlet works
   - Verify desk controller responds to height buttons

### Pairing Fails with Error

**Symptoms:**
- "Pairing failed" message
- Windows error during pairing
- Can see desk but can't connect

**Solutions:**

1. **Remove Previous Pairing**
   - Settings → Bluetooth & devices
   - Find desk in device list
   - Click three dots → Remove device
   - Wait 30 seconds
   - Retry pairing process

2. **Clear Bluetooth Cache**
   ```powershell
   # Run as Administrator
   Get-Service bthserv | Stop-Service
   Remove-Item -Path "HKLM:\SYSTEM\CurrentControlSet\Services\BTHPORT\Parameters\Keys" -Recurse -Force
   Get-Service bthserv | Start-Service
   ```
   - Restart computer
   - Retry pairing

3. **Try Different Pairing Method**
   - If modern settings fail, try legacy panel
   - If legacy fails, try modern settings
   - Some Windows installations work better with one method

## Movement Issues

### Desk Overshoots Target Height

**Symptoms:**
- Desk goes past configured height
- Stops above/below target
- Inconsistent stopping

**Solutions:**

1. **Adjust Units Till Stop**
   - Settings → Advanced → Units Till Stop
   - Increase value to stop earlier
   - Test with small adjustments
   - Default is usually optimal

2. **Recalibrate Heights**
   - Manually move to exact position
   - Use arrow keys in confirmation dialog
   - Save new precise height

### Desk Stops Short of Target

**Symptoms:**
- Desk doesn't reach configured height
- Stops too early
- Gap between actual and target

**Solutions:**

1. **Adjust Units Till Stop**
   - Settings → Advanced → Units Till Stop
   - Decrease value to stop later
   - Test incrementally

2. **Check for Physical Obstacles**
   - Ensure nothing blocking desk movement
   - Check cable management
   - Verify desk can reach height manually

### Desk Movement Is Jerky

**Symptoms:**
- Desk starts and stops
- Not smooth movement
- Unexpected pauses

**Solutions:**

1. **Check Bluetooth Signal**
   - Ensure strong signal
   - Move computer closer if needed
   - Remove interference sources

2. **Check Desk Hardware**
   - Test movement with physical buttons
   - If jerky physically, desk hardware issue
   - Contact Ikea support for hardware problems

### Stop Command Doesn't Work

**Symptoms:**
- Desk continues moving after Stop
- Stop command ignored
- No response to Stop

**Solutions:**

1. **Use Physical Buttons**
   - Press up or down on desk controller
   - This always stops movement
   - Hardware-level stop

2. **Check Connection**
   - Verify Bluetooth connected
   - Weak signal may delay commands

3. **Enable Parental Lock**
   - Settings → Advanced → Parental Lock
   - When enabled, physical buttons always stop
   - Adds extra safety layer

## Application Issues

### Application Won't Start

**Symptoms:**
- Double-click does nothing
- No icon appears in tray
- Process doesn't start

**Solutions:**

1. **Check Windows Version**
   - Requires Windows 10 or 11
   - Won't run on older versions

2. **Run as Administrator**
   - Right-click executable
   - Select "Run as administrator"
   - May be required for first run

3. **Check Antivirus**
   - Some antivirus blocks unknown executables
   - Add exception for Idasen application
   - Download from official GitHub releases only

4. **Reinstall Application**
   - Delete current executable
   - Download latest release
   - Extract to clean folder
   - Try running again

### Settings Not Saving

**Symptoms:**
- Changes reset after restart
- Positions not remembered
- Theme reverts to default

**Solutions:**

1. **Check File Permissions**
   - Settings stored in user profile
   - Ensure write permissions
   - Path shown in Settings → Advanced

2. **Check Disk Space**
   - Ensure sufficient disk space
   - Settings file is small but needs write space

3. **Run Without Admin Rights**
   - Don't run as administrator unless necessary
   - Settings save to user profile
   - Admin mode may use different profile

### Hotkeys Don't Work

**Symptoms:**
- Keyboard shortcuts not responding
- Hotkeys work in other apps
- No response to configured keys

**Solutions:**

1. **Check Hotkey Conflicts**
   - Another application may use same combination
   - Try different key combination
   - Test in different applications

2. **Restart Application**
   - Hotkey registration can fail
   - Restarting usually fixes it

3. **Check Hotkey Configuration**
   - Settings → Hot Keys
   - Verify combinations are saved
   - Reconfigure if needed

### System Tray Icon Missing

**Symptoms:**
- Can't find icon in tray
- Application running but no icon
- Process visible in Task Manager

**Solutions:**

1. **Check Hidden Icons**
   - Click up arrow in system tray
   - Icon may be in overflow area
   - Drag icon to main tray area

2. **Reset Tray Icons**
   - Task Manager → Processes
   - End "Windows Explorer"
   - File → Run new task → `explorer.exe`

## Performance Issues

### High CPU Usage

**Symptoms:**
- Application uses excessive CPU
- Computer slows down
- Fan runs constantly

**Solutions:**

1. **Check for Update Loop**
   - Disconnect from desk
   - If CPU normalizes, may be communication issue
   - Reconnect after restart

2. **Update Application**
   - Download latest release
   - Bug fixes may address issue

3. **Check Logs**
   - Excessive errors may cause CPU spike
   - See log file location in Settings

### High Memory Usage

**Symptoms:**
- Application uses a lot of RAM
- Memory leak suspected
- Usage increases over time

**Solutions:**

1. **Restart Application**
   - Temporary workaround
   - Exit and restart clears memory

2. **Report Issue**
   - If consistent, report on GitHub
   - Include logs and system info
   - Help developers fix the issue

## Log Files

### Accessing Logs

1. **Via Settings**
   - Open Settings → Advanced
   - Click Log Folder link
   - Opens folder in Explorer

2. **Manual Location**
   ```
   %LOCALAPPDATA%\IdasenSystemTray\Logs
   ```

### Reading Logs

Logs contain:
- Connection attempts and results
- Error messages
- Movement commands
- Configuration changes
- Timestamps for all events

**Note**: Sensitive data is masked in logs (addresses, identifiers)

### Useful Log Information

When reporting issues, include:
- Error messages
- Timestamp of issue
- Connection status
- Last successful operation

## Getting More Help

### Before Reporting an Issue

1. ✅ Check this troubleshooting guide
2. ✅ Review [FAQ](FAQ)
3. ✅ Search [existing issues](https://github.com/tschroedter/idasen-desk/issues)
4. ✅ Check [discussions](https://github.com/tschroedter/idasen-desk/discussions)
5. ✅ Review log files
6. ✅ Try basic solutions (restart, reconnect)

### Reporting a New Issue

When creating an issue, include:

**System Information:**
- Windows version (e.g., Windows 11 22H2)
- Application version
- Bluetooth adapter type

**Problem Description:**
- What you were trying to do
- What actually happened
- Expected behavior
- Steps to reproduce

**Logs:**
- Relevant error messages
- Timestamp of issue
- Connection status

**Attempted Solutions:**
- What you've already tried
- Results of each attempt

### Where to Get Help

- **GitHub Issues**: [Report bugs](https://github.com/tschroedter/idasen-desk/issues/new)
- **GitHub Discussions**: [Ask questions](https://github.com/tschroedter/idasen-desk/discussions)
- **FAQ**: [Common questions](FAQ)

### Community Support

- Search existing issues first
- Be respectful and patient
- Provide detailed information
- Follow up on questions
- Share solutions that worked

## Quick Reference

| Problem | Quick Fix |
|---------|-----------|
| Can't connect | Check pairing, restart app |
| Overshoots height | Adjust Units Till Stop |
| No tray icon | Check hidden icons |
| Settings not saving | Check permissions |
| Hotkeys not working | Check conflicts, restart |
| Slow connection | Configure Desk Address |
| Frequent disconnects | Check Bluetooth signal |

---

**Navigation**: [Home](Home) | [User Guide](User-Guide) | [Troubleshooting](Troubleshooting) | [FAQ](FAQ)
