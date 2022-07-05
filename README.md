# Ikea Idasen Desk
This repository is about controlling [Ikea's Idasen Desk](https://www.ikea.com/au/en/p/idasen-desk-sit-stand-black-dark-grey-s29280991/) using Windows 10 and BluetoothLE. Ikea only provides an Android and IOs app to control the desk. I thought it would be far more convenient to control the desk using a Windows 10 or 11. The installation instructions can be found at the end of this document.  

![taskbar](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/Taskbar.png)

---

# System Tray Application
At the moment the Windows 10 system tray application is a *work in progress*. It supports the following features:
- Detect Desk
- Show Settings
- Hide Settings
- Standing
- Seating
- Exit

![taskbar context menu](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/Taskbar%20Context%20Menu.png)

The application will automatically connect to the Idasen Desk during start-up. Pop-ups will notify you about the progress:
- Trying to connect,
- Connected or
- Failed to connect.

![Trying to connect](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/Trying%20To%20Connect.png)![Connected](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/Connected.png)![Connected](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/Failed%20to%20connect.png)

## Connect
*Connect* will try to detect a desk in case the application failed to detected the desk at start-up.

## Show Settings
*Show Settings* will display the current settings for the current user and allows to change them:
- Standing Height
- Seating Height
- Advanced Settings

_Note:_ The settings are stored per Windows user.

## Hide Settings
*Hide Setting* will close the settings window.

## General Settings
![settings](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/settings.PNG)

### Standing
*Standing* will move the desk to the standing height specified in the settings.

### Seating
*Seating* will move the desk to the seating height specified in the settings.

## Advanced Settings
![settingsadvanced](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/SettingsAdvanced.PNG)

### Desk Name
You can specify the desk name in case you changed it from the default name. By default the app is looking for a device/desk with a name starting with 'Desk'.

### Desk Address
If you know your devices ulong Bluetooth adress you can put it here. By default the app is looking for the Desk Name first and Desk Address second.

### Parental Lock
This feature allows to lock/unlock the physical desk controller. If this feature is enabled any pushing of the physical controller up or down will be immediately stop.

## Hot Keys
![settingsadvanced](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/SettingsHotKeys.PNG)
At the moment the application supports the following hot keys:
- Standing: _Ctrl + Shift + Alt + Cursor Up_
- Seating: _Ctrl + Shift + Alt + Cursor Down_

## Exit
*Exit* will close the application.

---
# Installation
## Windows 10
Download and run the self-contained application file: ![Idasen.SystemTray.exe](https://github.com/tschroedter/idasen-desk/releases/download/untagged-c4d4eb2fb3dc095f669a/Idasen.SystemTray.exe)

## Windows 11
Download and run the self-contained application file: ![Idasen.SystemTray.Win11.exe](https://github.com/tschroedter/idasen-desk/releases/download/untagged-e5698e6ca1062db1aec1/Idasen.SystemTray.Win11.exe)

## Additional step for Windows 10 and 11
Windows will show a pop-up window saying 'Windows protected your PC'. Please, click 'More info' and 'Run anyway' to start the application.

![WindowsProtectedYourPC1](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/WindowsProtectedYourPC1.png)

![WindowsProtectedYourPC2](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/WindowsProtectedYourPC2.png)


---

# Problems
_Q: The application fails to connect to the Idasen desk?_

A: Windows 10 needs to be connected to the Idasen desk which means the desk should be listed as a Bluetooth device.

![Trying to connect](https://github.com/tschroedter/idasen-desk/blob/main/docs/images/Windows%2010%20Bluetooth%20Settings.png)

