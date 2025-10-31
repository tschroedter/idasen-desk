# Getting Started

This guide will help you install and set up the Idasen Desk Controller on your Windows system.

## Prerequisites

Before you begin, ensure you have:

- ✅ **Windows 10 or 11** operating system
- ✅ **Bluetooth LE adapter** (built-in or USB)
- ✅ **Ikea Idasen standing desk**
- ✅ **Desk paired with Windows** via Bluetooth settings

## Installation Methods

### Method 1: Direct Download (Recommended)

The easiest way to get started is to download the standalone executable:

1. **Download the Latest Release**
   - Go to the [Latest Release](https://github.com/tschroedter/idasen-desk/releases/latest) page
   - Download `Idasen-SystemTray-[version]-win-x64.exe`
   - No .NET runtime installation required (self-contained)

2. **Run the Application**
   - Double-click the downloaded executable
   - If Windows SmartScreen appears:
     - Click "More info"
     - Click "Run anyway"
   - The application will start and appear in your system tray

3. **Configure Auto-Start (Optional)**
   - Press `Win + R` to open Run dialog
   - Type `shell:startup` and press Enter
   - Create a shortcut to the executable in this folder
   - The app will now start automatically when you log in

### Method 2: Scoop Package Manager

For users who prefer package managers:

1. **Install Scoop** (if not already installed)
   ```powershell
   # In PowerShell
   Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
   irm get.scoop.sh | iex
   ```

2. **Add the Custom Bucket**
   ```powershell
   scoop bucket add tschroedter https://github.com/tschroedter/scoop-bucket
   ```

3. **Install the Application**
   ```powershell
   scoop install tschroedter/idasen-systemtray
   ```

4. **Update the Application**
   ```powershell
   scoop update tschroedter/idasen-systemtray
   ```

## Pairing Your Desk

**Important**: Before using the application, you must pair your Idasen desk with Windows.

### Windows 11 Pairing

#### Option 1: Enable Advanced Discovery

1. Open **Settings** → **Bluetooth & devices**
2. Enable **"Show all Bluetooth devices"**
3. Put your desk in pairing mode:
   - Press and hold the pairing button on the desk controller
   - The LED will start blinking
4. Your desk should appear in the device list as "Desk" or "IDASEN"
5. Click to pair

For more details, see this [Reddit discussion](https://www.reddit.com/r/cricut/comments/14h9sz8/windows_11_bluetooth_issues_fixed).

#### Option 2: Use Legacy Devices Panel

If Option 1 doesn't work:

1. Open **Settings** → **Bluetooth & devices** → **Devices**
2. Scroll down and click **"More devices and printers settings"**
3. Right-click in empty space and select **"Add a device"**
4. Put desk in pairing mode
5. Select the desk from the list and click **"Next"**
6. Follow the on-screen instructions

### Windows 10 Pairing

1. Open **Settings** → **Devices** → **Bluetooth & other devices**
2. Click **"Add Bluetooth or other device"**
3. Select **"Bluetooth"**
4. Put your desk in pairing mode
5. Select the desk from the list
6. Click **"Done"**

### Verifying the Pairing

After pairing, verify the connection:

1. Open **Settings** → **Bluetooth & devices** (Win 11) or **Devices** (Win 10)
2. Look for "Desk" or your custom desk name
3. Status should show as "Paired" or "Connected"

![Windows Bluetooth Settings](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/Windows%2010%20Bluetooth%20Settings.png)

## First Launch

When you first run the application:

1. **Locate the System Tray Icon**
   - Look for the desk icon in your Windows system tray (bottom-right corner)
   - You may need to click the arrow to show hidden icons

2. **Automatic Connection**
   - The app will automatically try to connect to your desk
   - You'll see desktop notifications showing connection status
   - Initial connection may take a few seconds

3. **Configure Settings**
   - Right-click the system tray icon
   - Select **"Show Settings"** (or click the icon)
   - Set your preferred desk heights for:
     - Standing position
     - Seating position
     - Custom 1 position
     - Custom 2 position

4. **Test the Connection**
   - Right-click the system tray icon
   - Select one of the position commands (e.g., "Standing")
   - Confirm the action in the dialog
   - Your desk should move to the configured position

## Quick Configuration

### Setting Your Preferred Heights

1. **Move Your Desk Manually**
   - Use the physical desk controller to move to your ideal height
   
2. **Save the Position**
   - Open Settings from the system tray
   - Go to the **General** tab
   - Enter the current height in the desired position field
   - Click **Save** or the height will be saved automatically

3. **Alternative: Fine-tune in Dialog**
   - Trigger a move command
   - In the confirmation dialog, use arrow keys to adjust
   - The new height is automatically saved

### Configuring Hotkeys

The default global hotkeys are:
- **Standing**: `Ctrl + Shift + Alt + ↑` (Up Arrow)
- **Seating**: `Ctrl + Shift + Alt + ↓` (Down Arrow)
- **Custom 1**: `Ctrl + Shift + Alt + ←` (Left Arrow)
- **Custom 2**: `Ctrl + Shift + Alt + →` (Right Arrow)

## Next Steps

Now that you're set up:

- 📖 Read the **[User Guide](User-Guide)** for detailed usage instructions
- 🎨 Explore **[Configuration](Configuration)** options for customization
- ❓ Check the **[FAQ](FAQ)** for common questions
- 🔧 Visit **[Troubleshooting](Troubleshooting)** if you encounter issues

## Troubleshooting Installation

### Windows SmartScreen Warning

This is normal for downloaded applications. The app is safe and open-source. Click "More info" → "Run anyway".

### Desk Not Found

If the app can't find your desk:
1. Verify the desk is paired in Windows Bluetooth settings
2. Ensure no other app is connected to the desk
3. Try disconnecting and reconnecting in Bluetooth settings
4. Check the application logs in Advanced Settings

### Connection Fails

See the **[Troubleshooting](Troubleshooting)** page for detailed solutions.

---

**Navigation**: [Home](Home) | [Getting Started](Getting-Started) | [User Guide](User-Guide) | [Troubleshooting](Troubleshooting)
