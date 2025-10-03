# !!!This document will be updated soon!!!

[![.NET CI](https://github.com/tschroedter/idasen-desk/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/tschroedter/idasen-desk/actions/workflows/dotnet-ci.yml)
[![CodeQL](https://github.com/tschroedter/idasen-desk/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/tschroedter/idasen-desk/actions/workflows/github-code-scanning/codeql)

# Latest Changes
- Updated .NET 4.8 to .NET 8.0
- Updated used NuGet Packages to latest
- Updated UI to look more like native Windows application
- Updated notification to use Windows build in notifcations
- Added support for themes
- Remember last desk height
- Fixed vulnerabilities in external NuGet packages by updating to latest version

# Ikea Idasen Desk
This repository is about controlling [Ikea's Idasen Desk](https://www.ikea.com/au/en/p/idasen-desk-sit-stand-black-dark-grey-s29280991/) using Windows 10/11 and BluetoothLE. Ikea only provides an Android and IOs app to control the desk. I thought it would be far more convenient to control the desk using a Windows 10/11. The [installation instructions](#Installation) can be found at the end of this document.

## System Tray Icon
The icon below is shown if the application is running and the desk height is not known.
![taskbar](https://github.com/tschroedter/idasen-desk/blob/main-face-lift/docs/images/V0.0.222/System_Tray_Icon_Unknow_Height.png)

The icon is updated to show the current desk height as soon as the desk is moving.
![taskbar](https://github.com/tschroedter/idasen-desk/blob/main-face-lift/docs/images/V0.0.222/System_Tray_Icon_Know_Height.png)

---

# System Tray Application
At the moment the updated UI for the Windows 10/11 system tray application is a *work in progress*. It supports the following features:
- Show Settings
- Hide Settings
- Connect
- Disconnect
- Standing
- Seating
- Stop
- Exit

![taskbar context menu](https://github.com/tschroedter/idasen-desk/blob/main-face-lift/docs/images/V0.0.222/System_Tray_Context_Menu.png)

The application will automatically connect to the Idasen Desk during start-up. Notifications will notify you about the progress:
- Trying to connect,
- Connected or
- Failed to connect.

![Notifications](https://github.com/tschroedter/idasen-desk/blob/main-face-lift/docs/images/V0.0.222/Notifications.png)

# Actions
The desk can be controlled by using the system tray context menu or the menu options on the left hand side when the settings menu is visible. To execute an action *double-click* the menu item and confirm the action in the shown dialog.
- Connect
- Disconnect
- Standing
- Seating
- Stop _(Stop the moving desk)_
- Hide Settings _(Hide/Close the window)_
- Exit _(Exit the application)_

# Settings
*Show Settings* will display the current settings for the current user and allows to change them. The following options are available:
- Advanced Settings
- Appearance
- General Settings
- Hot Keys

_Note:_ The settings are stored per Windows user.

## Advanced Settings
![settingsadvanced](https://github.com/tschroedter/idasen-desk/blob/main-face-lift/docs/images/V0.0.222/Settins_Advanced.png)

### Log Folder
This show the location of the log files.

### Settings File
This shows the location of the settings file.

### Desk Name
You can specify the desk name in case you changed it from the default name. By default the app is looking for a device/desk with a name starting with 'Desk'.

### Desk Address
If you know your devices ulong Bluetooth adress you can put it here. By default the app is looking for the Desk Name first and Desk Address second.

### Parental Lock
This feature allows to lock/unlock the physical desk controller. If this feature is enabled any pushing of the physical controller up or down will be immediately stop.

## Appearance
![settings](https://github.com/tschroedter/idasen-desk/blob/main-face-lift/docs/images/V0.0.222/Settins_Appearance.png)

### Theme
This allows to switch between the different themes.

## General Settings
![settings](https://github.com/tschroedter/idasen-desk/blob/main-face-lift/docs/images/V0.0.222/Settings_General.png)

### Standing
*Standing* will move the desk to the standing height specified in the settings.

### Seating
*Seating* will move the desk to the seating height specified in the settings.

## Hot Keys
![settingsadvanced](https://github.com/tschroedter/idasen-desk/blob/main-face-lift/docs/images/V0.0.222/Settins_Hotkeys.png)
At the moment the application supports the following hot keys:
- Standing: _Ctrl + Shift + Alt + Cursor Up_
- Seating: _Ctrl + Shift + Alt + Cursor Down_

---
# Installation
## Windows 10
Download and run the self-contained application file: [Idasen.SystemTray.Win11.exe](https://github.com/tschroedter/idasen-desk/releases/download/V0.0.263/Idasen.Desk.exe)

## Windows 11
Download and run the self-contained application file: [Idasen.SystemTray.Win11.exe](https://github.com/tschroedter/idasen-desk/releases/download/V0.0.263/Idasen.Desk.exe)

---
### Trouble Pairing the desk?
## Solution 1
User Evo589 had problems getting Windows 11 to even see the desk in pairing mode. Turns out you need to set the Bluetooth device discovery setting to advanced [more details](https://www.reddit.com/r/cricut/comments/14h9sz8/windows_11_bluetooth_issues_fixed).

## Solution 2
In case Windows 11 can't find the desk when you try to add/find a new Bluetooth device you can try the following:
- Open the 'Bluetooth & devices > Devices' screen.
![Advanced settings](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/Win11_Devices.png)
- Scroll down a bit and click 'More devices and printer settings'.
- The 'Devices and Printers' window opens.
![Add a device](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/Win11_Add_Devices_Printers.png)
- Open the context menu by clicking anywhere on the background.
- Select 'Add devices and printers.
- The 'Choose a device or printer to add to this PC' window opens.
![Choose a device](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/Win11_Choose_A_Device.png)
- Put the Idasen Desk into Bluetooth pairing mode.
- The desk appears in the window.
- Select 'Next' and follow the instructions.

## Any problems?
Let me know if you have any trouble installing or using the application.

---
# Problems
_Q: The application fails to connect to the Idasen desk?_

A: Windows 10 needs to be connected to the Idasen desk which means the desk should be listed as a Bluetooth device.

![Trying to connect](https://github.com/tschroedter/idasen-desk/blob/main-face-lift/docs/images/Windows%2010%20Bluetooth%20Settings.png)

