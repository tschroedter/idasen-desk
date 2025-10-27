# Ikea Idasen Desk - Windows Controller

[![Build and Test (Draft Release)](https://github.com/tschroedter/idasen-desk/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/tschroedter/idasen-desk/actions/workflows/dotnet-ci.yml)
[![CodeQL](https://github.com/tschroedter/idasen-desk/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/tschroedter/idasen-desk/actions/workflows/github-code-scanning/codeql)
[![Release Drafter](https://github.com/tschroedter/idasen-desk/actions/workflows/release-drafter.yml/badge.svg)](https://github.com/tschroedter/idasen-desk/actions/workflows/release-drafter.yml)
[![SonarCloud Analysis](https://github.com/tschroedter/idasen-desk/actions/workflows/sonarcloud.yml/badge.svg)](https://github.com/tschroedter/idasen-desk/actions/workflows/sonarcloud.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=tschroedter_idasen-desk&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=tschroedter_idasen-desk)
[![Dependabot Status](https://img.shields.io/badge/Dependabot-enabled-success?logo=dependabot)](https://github.com/tschroedter/idasen-desk/network/updates)

A Windows 10/11 desktop application for controlling [Ikea's Idasen standing desk](https://www.ikea.com/au/en/p/idasen-desk-sit-stand-black-dark-grey-s29280991/) via Bluetooth LE. While Ikea provides Android and iOS apps, this application brings convenient desk control directly to your Windows desktop through a system tray interface.

## Table of Contents

- [What's New](#whats-new)
- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
  - [Direct Download](#direct-download)
  - [Scoop Package Manager](#scoop-package-manager)
- [Usage](#usage)
  - [System Tray Icon](#system-tray-icon)
  - [Context Menu](#context-menu)
  - [Actions](#actions)
- [Settings](#settings)
  - [General Settings](#general-settings)
  - [Hot Keys](#hot-keys)
  - [Appearance](#appearance)
  - [Advanced Settings](#advanced-settings)
- [Troubleshooting](#troubleshooting)
  - [Pairing the Desk](#pairing-the-desk)
  - [Connection Issues](#connection-issues)
- [Contributing](#contributing)
- [License](#license)

## What's New

### Version 0.1.250
Short description: This patch release focuses on stability and quality-of-life improvements. It includes several bug fixes, performance optimizations, small usability improvements, dependency/security updates, and adds SonarCloud scanning so all code is now continuously checked for quality and security.

- 🛡️Fixes: resolved multiple reported crashes and edge-case bugs.
- 📡Improvements: reduced memory/CPU usage in core processing paths and improved logging for easier troubleshooting.
- 📊 Quality: all code is now scanned by SonarCloud (static analysis for bugs, code smells, and security issues).
- 📦 Maintenance: updated third-party dependencies and applied security patches.

### Version 0.1.182

- ✨ Added 2 new desk positions: Custom 1 and Custom 2
- ⚙️ Desk positions can be fine-tuned using up/down icons or arrow keys in the confirmation dialog
- 🎨 Enhanced UI for the system tray application
- 👁️ Configurable visibility for desk positions and Stop command in context menu
- 🔌 Smart context menu: shows only 'Connect' or 'Disconnect' based on connection state
- 🖱️ Click system tray icon to show/hide settings window
- 🪟 Resizable settings window with improved window management
- 📡 Improved Bluetooth LE connection handling
- 📦 Updated all NuGet packages to latest versions
- 🔒 Masked sensitive data in log files
- 🛡️ Fixed security vulnerabilities through package updates
- 📊 Added SonarCloud code quality analysis
- 🐛 Fixed various code smells and bugs identified by SonarCloud

[View full changelog](CHANGELOG.md)

## Features

### Core Features

- 🖥️ **System Tray Integration**: Unobtrusive control directly from your Windows taskbar
- 🎯 **Preset Positions**: Four customizable height positions (Standing, Seating, Custom 1, Custom 2)
- ⌨️ **Global Hotkeys**: Control your desk from any application
- 🔄 **Auto-Connect**: Automatically connects to your desk on startup
- 📏 **Fine-Tuning**: Adjust positions using arrow keys during confirmation
- 🔔 **Desktop Notifications**: Stay informed about connection status and desk movement
- 🎨 **Theme Support**: Multiple color themes to match your desktop
- 🔒 **Parental Lock**: Lock the physical desk controller to prevent unwanted adjustments
- ⚙️ **Per-User Settings**: Each Windows user can have their own desk configurations

### Supported Operations

- Connect/Disconnect from desk
- Move to preset heights
- Stop desk movement
- Configure and manage settings
- Lock/unlock physical desk controller

## Requirements

- **Operating System**: Windows 10/11
- **Hardware**: Bluetooth LE adapter
- **Desk**: Ikea Idasen standing desk
- **Pairing**: Desk must be paired with Windows via Bluetooth settings

## Installation

### Direct Download

Download the latest self-contained executable (no .NET runtime installation required):

**[Download Idasen-SystemTray-0.1.182-win-x64.exe](https://github.com/tschroedter/idasen-desk/releases/download/v0.1.182/Idasen-SystemTray-0.1.182-win-x64.exe)**

1. Download the executable from the link above
2. Run the executable
3. If Windows shows a security warning, click "More info" and then "Run anyway"
4. The application will start and appear in your system tray

### Scoop Package Manager

For users who prefer package managers, you can install via [Scoop](https://scoop.sh/):

```powershell
scoop bucket add tschroedter https://github.com/tschroedter/scoop-bucket
scoop install tschroedter/idasen-systemtray
```

And updating the app:
```powershell
scoop update tschroedter/idasen-systemtray
```

_Note: The app doesn't have enough stars to be in the official Scoop repository, so we maintain our own bucket ([PR #20](https://github.com/tschroedter/idasen-desk/issues/20))._

## Usage

### System Tray Icon

The application runs in your system tray and provides visual feedback about your desk's current state.

**Unknown Height** - Displayed when the desk height hasn't been determined yet:

![System Tray Icon - Unknown Height](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/V0.0.222/System_Tray_Icon_Unknow_Height.png)

**Known Height** - Updated automatically when the desk moves:

![System Tray Icon - Known Height](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/V0.0.222/System_Tray_Icon_Know_Height.png)

**Tip**: Click the system tray icon to show/hide the settings window.

### Context Menu

Right-click the system tray icon to access the context menu:

![System Tray Context Menu](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/V0.1.77/System_Tray_Context_Menu.png)

Available options:
- **Show/Hide Settings** - Toggle the settings window
- **Connect/Disconnect** - Manage Bluetooth connection (only the relevant option is shown)
- **Standing** - Move to standing position
- **Seating** - Move to seating position
- **Custom 1** - Move to custom position 1
- **Custom 2** - Move to custom position 2
- **Stop** - Immediately stop desk movement
- **Exit** - Close the application

_Note: Menu items can be shown/hidden in the settings._

### Actions

The desk can be controlled through:
1. **System tray context menu** (right-click)
2. **Settings window menu** (left sidebar)
3. **Global hotkeys** (configurable)

To execute an action:
1. Double-click a menu item (or use the hotkey)
2. A confirmation dialog appears
3. Fine-tune the position using up/down arrow keys if needed
4. Confirm or cancel the action

### Automatic Connection

The application automatically connects to your Idasen desk on startup. Desktop notifications keep you informed:

![Connection Notifications](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/V0.0.222/Notifications.png)

- 🔄 Trying to connect...
- ✅ Connected
- ❌ Failed to connect

## Settings

Access settings by clicking "Show Settings" from the context menu or clicking the system tray icon. All settings are stored per Windows user.

### General Settings

![General Settings](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/V0.1.77/Settings_General.png)

#### Desk Positions

Configure up to four preset positions:

- **Standing**: Your preferred standing height
- **Seating**: Your preferred sitting height  
- **Custom 1**: Additional custom position
- **Custom 2**: Additional custom position

Each position includes:
- Height value (adjustable)
- **Tray** checkbox: Show/hide in context menu

#### Tray Menu Options

- **Stop**: Toggle visibility of the Stop command in the context menu

### Hot Keys

![Hotkey Settings](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/V0.1.77/Settins_Hotkeys.png)

Default global hotkeys (works in any application):

- **Standing**: `Ctrl + Shift + Alt + ↑` (Up Arrow)
- **Seating**: `Ctrl + Shift + Alt + ↓` (Down Arrow)
- **Custom 1**: `Ctrl + Shift + Alt + ←` (Left Arrow)
- **Custom 2**: `Ctrl + Shift + Alt + →` (Right Arrow)

### Appearance

![Appearance Settings](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/V0.0.222/Settins_Appearance.png)

#### Theme

Choose from multiple color themes to match your desktop environment.

### Advanced Settings

![Advanced Settings](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/V0.0.222/Settins_Advanced.png)

#### Log Folder

Displays the location of application log files for troubleshooting.

#### Settings File

Shows where your user settings are stored.

#### Desk Name

Specify a custom desk name if you've changed it from the default. The app searches for devices with names starting with "Desk" by default.

#### Desk Address

Optionally specify your desk's Bluetooth MAC address (unsigned long format) for faster connection. The app prioritizes Desk Name first, then Desk Address.

#### Parental Lock

Enable this feature to lock the physical desk controller. When active, pressing the up/down buttons on the desk will immediately stop movement, preventing unauthorized adjustments.

#### Units Till Stop

_(Coming Soon)_ Fine-tune the estimated stopping distance from maximum movement speed. Adjust this if your desk overshoots or undershoots target heights.

#### Reset Settings

Reset all settings to their default values.

_Note: The settings are stored per Windows user._

## Troubleshooting

### Pairing the Desk

Before using this application, you must pair your Idasen desk with Windows through Bluetooth settings.

#### Solution 1: Enable Advanced Bluetooth Discovery (Windows 11)

Some users have reported that Windows 11 doesn't detect the desk in pairing mode. The solution is to enable advanced Bluetooth device discovery:

1. Open Windows Settings → Bluetooth & devices
2. Enable "Show all Bluetooth devices"
3. Put your desk in pairing mode
4. The desk should now appear in the device list

For more details, see this [Reddit discussion](https://www.reddit.com/r/cricut/comments/14h9sz8/windows_11_bluetooth_issues_fixed).

#### Solution 2: Use Legacy Devices and Printers

If Windows 11 still can't find your desk using the modern Bluetooth settings:

1. **Open Bluetooth Settings**
   - Go to Settings → Bluetooth & devices → Devices
   
   ![Windows 11 Devices](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/Win11_Devices.png)

2. **Access Legacy Settings**
   - Scroll down and click "More devices and printers settings"
   - The "Devices and Printers" window opens
   
   ![Add a device](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/Win11_Add_Devices_Printers.png)

3. **Add Device**
   - Right-click in the window background (empty space)
   - Select "Add a device"
   - The "Choose a device or printer to add to this PC" window opens
   
   ![Choose a device](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/Win11_Choose_A_Device.png)

4. **Pair the Desk**
   - Put your Idasen desk into Bluetooth pairing mode
   - The desk should appear in the window
   - Select it and click "Next"
   - Follow the on-screen instructions

### Connection Issues

**Q: The application fails to connect to the Idasen desk?**

**A:** Ensure your desk is paired with Windows first. The desk should appear in your Bluetooth devices list.

To verify:
1. Open Settings → Bluetooth & devices
2. Check if "Desk" or your custom desk name is listed
3. The desk should show as "Connected" or "Paired"

![Windows 10 Bluetooth Settings](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/Windows%2010%20Bluetooth%20Settings.png)

If the desk is paired but the application still can't connect:
- Try disconnecting and reconnecting the desk in Windows Bluetooth settings
- Restart the application
- Check the log files (location shown in Advanced Settings)
- Ensure no other application is using the desk's Bluetooth connection

**Still having problems?** Please [open an issue](https://github.com/tschroedter/idasen-desk/issues) with:
- Your Windows version
- Error messages from log files
- Steps you've already tried

## Contributing

We welcome contributions to improve this project! Whether you're fixing bugs, adding features, or improving documentation, your help is appreciated.

### For Contributors

When submitting a pull request:

1. **Follow the code style guidelines** - See [GitHub Copilot Instructions](.github/copilot-instructions.md) for detailed standards
2. **Add appropriate labels** - Required for our automated changelog system:
   - `feature` or `enhancement` - New features
   - `fix`, `bugfix`, or `bug` - Bug fixes
   - `chore` or `maintenance` - Maintenance tasks
   - `documentation` or `docs` - Documentation changes
3. **Include tests** - Add or update tests for new functionality
4. **Update documentation** - Keep docs in sync with code changes

### Development Resources

- **[GitHub Copilot Instructions](.github/copilot-instructions.md)** - Comprehensive development guide including:
  - Building and testing instructions
  - Code style and standards
  - Pull request guidelines
  - Common development tasks
  
- **[Changelog Automation Guide](docs/CHANGELOG_AUTOMATION.md)** - How our automated changelog works
- **[Implementation Details](docs/IMPLEMENTATION_SUMMARY.md)** - Technical implementation overview
- **[Workflow Diagrams](docs/WORKFLOW_DIAGRAM.md)** - Visual representation of CI/CD processes

### Quick Start for Developers

```bash
# Clone the repository
git clone https://github.com/tschroedter/idasen-desk.git
cd idasen-desk/src

# Restore dependencies
dotnet restore Idasen-Desk.sln

# Build the solution
dotnet build Idasen-Desk.sln --configuration Release

# Run tests
dotnet test Idasen-Desk.sln --configuration Release
```

### Tech Stack

- **Language**: C# / .NET 8.0
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Architecture**: MVVM with Autofac DI
- **Testing**: xUnit, NSubstitute, FluentAssertions
- **Communication**: Bluetooth LE

### Getting Help

- Check the [documentation](docs/) for detailed guides
- Review existing [issues](https://github.com/tschroedter/idasen-desk/issues)
- Use the [pull request template](.github/pull_request_template.md)
- View [CI/CD workflows](.github/workflows/) in the Actions tab

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## Acknowledgments

- Built with ❤️ for the Ikea Idasen desk community
- Thanks to all [contributors](https://github.com/tschroedter/idasen-desk/graphs/contributors)

## Links

- **[Releases](https://github.com/tschroedter/idasen-desk/releases)** - Download the latest version
- **[Issues](https://github.com/tschroedter/idasen-desk/issues)** - Report bugs or request features
- **[Changelog](CHANGELOG.md)** - View all changes
- **[Ikea Idasen Desk](https://www.ikea.com/au/en/p/idasen-desk-sit-stand-black-dark-grey-s29280991/)** - Official product page

