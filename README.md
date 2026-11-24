# Ikea Idasen Desk - Windows Controller

[![Build and Test (Draft Release)](https://github.com/tschroedter/idasen-desk/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/tschroedter/idasen-desk/actions/workflows/dotnet-ci.yml)
[![CodeQL](https://github.com/tschroedter/idasen-desk/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/tschroedter/idasen-desk/actions/workflows/github-code-scanning/codeql/badge.svg)
[![SonarCloud Analysis](https://github.com/tschroedter/idasen-desk/actions/workflows/sonarcloud.yml/badge.svg)](https://github.com/tschroedter/idasen-desk/actions/workflows/sonarcloud.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=tschroedter_idasen-desk&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=tschroedter_idasen-desk)
[![Dependabot Status](https://img.shields.io/badge/Dependabot-enabled-success?logo=dependabot)](https://github.com/tschroedter/idasen-desk/network/updates)

A Windows 10/11 desktop application for controlling [Ikea's Idasen standing desk](https://www.ikea.com/au/en/p/idasen-desk-sit-stand-black-dark-grey-s29280991/) via Bluetooth LE. The app runs unobtrusively in the Windows system tray.

> 📚 **[Visit the Wiki](https://github.com/tschroedter/idasen-desk/wiki)** for comprehensive documentation, guides, and troubleshooting.

This project uses the [idasen-desk-core](https://github.com/tschroedter/idasen-desk-core) library for all Bluetooth communication with the Idasen desk.

## Table of Contents

- [Thank You](#thank-you)
- [What's New](#whats-new)
- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
  - [Direct Download](#direct-download)
  - [Scoop Package Manager](#scoop-package-manager)
  - [Build from Source](#build-from-source)
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
  - [Known Limitations](#known-limitations)
- [Contributing](#contributing)
- [License](#license)

## Thank You

A special thank you to all the amazing people who test this application and provide valuable feedback! Your contributions help make this app better for everyone.

## What's New

### Latest published release

- **v0.1.540**: Added an update check at startup and every 24 hours. The app notifies users when a new version is available.
- See [CHANGELOG.md](CHANGELOG.md) for complete release history and details.

[View full changelog](CHANGELOG.md)

## Features

### Core Features

- 🖥️ **System Tray Integration**: Unobtrusive control directly from your Windows taskbar
- 🎯 **Preset Positions**: Four customizable height positions (Standing, Seating, Custom 1, Custom 2)
- ⌨️ **Configurable Global Hotkeys**: Control your desk from any application using fully customizable hotkeys
- 🔄 **Auto-Connect**: Automatically connects to your desk on startup
- 📏 **Fine-Tuning**: Adjust positions using arrow keys during confirmation
- 🔔 **Desktop Notifications**: Stay informed about connection status, desk movement, and new application versions
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

- **Operating System (app runtime)**: Windows 10/11
- **Hardware**: Bluetooth LE adapter
- **Desk**: Ikea Idasen standing desk
- **Pairing**: Desk must be paired with Windows via Bluetooth settings
- **For local development**: .NET SDK 10.0+

## Installation

### Direct Download

Download the latest self-contained executable (no .NET runtime installation required):

**[Download Latest Release Asset](https://github.com/tschroedter/idasen-desk/releases/latest)**

1. Download the executable from the link above
2. Run the executable
3. If Windows shows a security warning, click **More info** and then **Run anyway**
4. The application will start and appear in your system tray

### Scoop Package Manager

For users who prefer package managers, you can install via [Scoop](https://scoop.sh/):

```powershell
scoop bucket add tschroedter https://github.com/tschroedter/scoop-bucket
scoop install tschroedter/idasen-systemtray
```

To update the app:

```powershell
scoop update tschroedter/idasen-systemtray
```

_Note: The app doesn't have enough stars to be in the official Scoop repository, so we maintain our own bucket ([PR #20](https://github.com/tschroedter/idasen-desk/issues/20))._

### Build from Source

```bash
cd src
dotnet restore Idasen-Desk.sln
dotnet build Idasen-Desk.sln --configuration Release
dotnet test Idasen-Desk.sln --configuration Release
```

Publish a self-contained executable:

```bash
cd src
dotnet publish Idasen.SystemTray.Win11/Idasen.SystemTray.Win11.csproj \
  --configuration Release \
  --runtime win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishReadyToRun=true
```

## Usage

### System Tray Icon

The application runs in your system tray and provides visual feedback about your desk's current state.

**Tip**: Click the system tray icon to show/hide the settings window.

### Context Menu

Right-click the system tray icon to access the context menu:

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

- 🔄 Trying to connect...
- ✅ Connected
- ❌ Failed to connect

If an existing Bluetooth session becomes stale, the app attempts to recover by reconnecting automatically.

### Update Notifications

The app checks for a new release at startup and every 24 hours. If an update is available, it notifies you so you can download the latest version.

## Settings

Access settings by clicking "Show Settings" from the context menu or clicking the system tray icon. All settings are stored per Windows user.

### General Settings

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

Global hotkeys are fully configurable through the **Settings → Hot Keys** page. For each desk position you can:

- **Enable or disable** all global hotkeys using the toggle at the top of the Hot Keys page
- **Choose a key** (F-keys, arrow keys, numpad, letters, etc.)
- **Choose modifiers** (Ctrl, Alt, Shift, Win — any combination)

Default global hotkeys (works in any application):

- **Standing**: `Ctrl + Shift + Alt + ↑` (Up Arrow)
- **Seating**: `Ctrl + Shift + Alt + ↓` (Down Arrow)
- **Custom 1**: `Ctrl + Shift + Alt + ←` (Left Arrow)
- **Custom 2**: `Ctrl + Shift + Alt + →` (Right Arrow)

For advanced configuration (e.g. editing `Settings.json` directly), see the **[Hotkey Customization Guide](docs/HOTKEY_CUSTOMIZATION.md)**.

### Appearance

#### Theme

Choose from multiple color themes to match your desktop environment.

### Advanced Settings

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

Fine-tune the estimated stopping distance from maximum movement speed. Adjust this if your desk overshoots or undershoots target heights.

#### Reset Settings

Reset all settings to their default values.

_Note: The settings are stored per Windows user._

## Troubleshooting

### Pairing the Desk

Before using this application, you must pair your Idasen desk with Windows through Bluetooth settings.

If Windows 11 cannot find the desk in pairing mode, enable advanced Bluetooth discovery under **Settings → Bluetooth & devices → Show all Bluetooth devices**.

If Windows 11 still cannot find the desk using the modern Bluetooth settings:
1. Open **Settings → Bluetooth & devices → Devices**
2. Scroll down and open **More devices and printers settings**
3. Right-click in the Devices and Printers window and select **Add a device**
4. Put your desk into Bluetooth pairing mode
5. Select the desk and follow the Windows pairing instructions

### Connection Issues

**Q: The application fails to connect to the Idasen desk?**

**A:** Ensure your desk is paired with Windows first. The desk should appear in your Bluetooth devices list.

To verify:
1. Open Settings → Bluetooth & devices
2. Check whether "Desk" or your custom desk name is listed
3. The desk should show as "Connected" or "Paired"

If the desk is paired but the application still cannot connect:
- Try disconnecting and reconnecting the desk in Windows Bluetooth settings
- Restart the application
- Check the log files (location shown in Advanced Settings)
- Ensure no other application is using the desk's Bluetooth connection
- Wait briefly and retry; recent versions include stale connection detection and automatic reconnection.

**Still having problems?** Please [open an issue](https://github.com/tschroedter/idasen-desk/issues) with:
- Your Windows version
- Error messages from log files
- Steps you've already tried

### Known Limitations

- Windows-only application (WPF + Windows Bluetooth stack).
- The desk must already be paired in Windows before first use.
- Only one controller can actively control the desk over Bluetooth at a time.

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

- **[GitHub Copilot Instructions](.github/copilot-instructions.md)** - Comprehensive development guide
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

> Note: The test project targets `net10.0-windows10.0.19041`, so tests require a Windows desktop runtime environment.

### CI/CD and Automation Workflows

- **Build and Test (Draft Release)**: `.github/workflows/dotnet-ci.yml` builds, tests, publishes a versioned `win-x64` single-file executable, tags, and drafts releases.
- **Release Drafter**: `.github/workflows/release-drafter.yml` updates draft release notes on pushes to `main`.
- **Update Changelog**: `.github/workflows/update-changelog.yml` updates `CHANGELOG.md` when a release is published.
- **SonarCloud**: `.github/workflows/sonarcloud.yml` performs static analysis.

### Tech Stack

- **Language**: C# / .NET 10.0
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Architecture**: MVVM with Autofac DI
- **Testing**: xUnit, NSubstitute, FluentAssertions
- **Communication**: Bluetooth LE

### Getting Help

- Check the [documentation](docs/) for detailed guides
- Review existing [issues](https://github.com/tschroedter/idasen-desk/issues)
- Use the [pull request template](.github/pull_request_template.md)
- View [CI/CD workflows](.github/workflows/) in the Actions tab

## Documentation

### 📚 Wiki

Comprehensive documentation is available in the [**Wiki**](https://github.com/tschroedter/idasen-desk/wiki):

**For Users:**
- [Getting Started](https://github.com/tschroedter/idasen-desk/wiki/Getting-Started)
- [User Guide](https://github.com/tschroedter/idasen-desk/wiki/User-Guide)
- [Troubleshooting](https://github.com/tschroedter/idasen-desk/wiki/Troubleshooting)
- [FAQ](https://github.com/tschroedter/idasen-desk/wiki/FAQ)
- [Configuration](https://github.com/tschroedter/idasen-desk/wiki/Configuration)

**For Developers:**
- [Developer Guide](https://github.com/tschroedter/idasen-desk/wiki/Developer-Guide)
- [Build Instructions](https://github.com/tschroedter/idasen-desk/wiki/Build-Instructions)
- [Architecture](https://github.com/tschroedter/idasen-desk/wiki/Architecture)

### 📖 Additional Resources

- **[Hotkey Customization Guide](docs/HOTKEY_CUSTOMIZATION.md)**
- **[Changelog Automation Guide](docs/CHANGELOG_AUTOMATION.md)**
- **[Implementation Details](docs/IMPLEMENTATION_SUMMARY.md)**
- **[Workflow Diagrams](docs/WORKFLOW_DIAGRAM.md)**
- **[SonarCloud Setup](docs/SONARCLOUD_SETUP.md)**

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) for details.

---

## Acknowledgments

- Built with ❤️ for the Ikea Idasen desk community
- Thanks to all [contributors](https://github.com/tschroedter/idasen-desk/graphs/contributors)

## Links

- **[Wiki](https://github.com/tschroedter/idasen-desk/wiki)** - Comprehensive documentation
- **[Releases](https://github.com/tschroedter/idasen-desk/releases)** - Download the latest version
- **[Issues](https://github.com/tschroedter/idasen-desk/issues)** - Report bugs or request features
- **[Discussions](https://github.com/tschroedter/idasen-desk/discussions)** - Ask questions and share ideas
- **[Changelog](CHANGELOG.md)** - View all changes
- **[Ikea Idasen Desk](https://www.ikea.com/au/en/p/idasen-desk/s29280991/)** - Official product page
