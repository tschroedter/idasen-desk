# Ikea Idasen Desk
This repository is about controlling [Ikea's Idasen Desk](https://www.ikea.com/au/en/p/idasen-desk-sit-stand-black-dark-grey-s29280991/) using Windows 10 and BluetoothLE. Ikea only provides an Android and IOs app to control the desk. I thought it would be far more convenient to control the desk using a Windows 10 system tray application.  

## System Tray Application
At the moment the Windows 10 system tray application is a work in progress (branch *system_tray*). It supports the following features:
- Detect Desk
- Show Settings
- Hide Settings
- Standing
- Seating
- Exit

### Detect Desk
Detec Desk will try to detect the desk again in case the application failed to detected the desk at start-up.

### Show Settings
Show Settings will display the current settings and allows to change them:
- Standing Height
- Seating Height

### Hide Settings
Hide Setting will close the settings window.

### Standing
Standing will move the desk to the standing height specified in the settings.

### Seating
Seating will move the desk to the seating height specified in the settings.

### Exit
Will close the application.







