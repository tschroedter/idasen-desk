# Frequently Asked Questions (FAQ)

Common questions and answers about the Idasen Desk Controller.

## General Questions

### What is the Idasen Desk Controller?

The Idasen Desk Controller is a Windows 10/11 desktop application that allows you to control your Ikea Idasen standing desk via Bluetooth LE. It provides a convenient system tray interface with features like preset heights, global hotkeys, and automatic connection management.

### Why do I need this when Ikea has a mobile app?

While Ikea provides Android and iOS apps, they don't offer a Windows application. This project fills that gap, allowing you to control your desk directly from your Windows computer without needing to reach for your phone. Features like global hotkeys make it even more convenient than mobile apps.

### Is this official software from Ikea?

No, this is an independent open-source project and is not affiliated with or endorsed by Ikea. It's developed by the community for the community.

### Is it safe to use?

Yes! The application is:
- **Open source** - All code is publicly available for review
- **Well tested** - Includes comprehensive unit tests
- **Continuously analyzed** - Code quality checked by SonarCloud
- **Community reviewed** - Many users have tested and contributed

However, as with any third-party software, use at your own risk. See the [MIT License](https://github.com/tschroedter/idasen-desk/blob/main/LICENSE) for details.

### Does it cost anything?

No, the application is completely free and open source under the MIT License.

## Compatibility Questions

### Which desks are compatible?

The application is designed for Ikea's Idasen standing desks that support Bluetooth LE control. This includes:
- Idasen Desk with electronic height adjustment
- Desks with the Bluetooth-enabled control panel

If your desk has the Ikea mobile app compatibility, it should work with this application.

### Which Windows versions are supported?

- ✅ Windows 11 (all versions)
- ✅ Windows 10 (all versions)
- ❌ Windows 8.1 and earlier (not supported)

### Do I need a Bluetooth adapter?

Yes, you need a Bluetooth LE (Bluetooth Low Energy) capable adapter. Most modern Windows computers have this built-in. If yours doesn't, you can purchase a USB Bluetooth adapter (ensure it supports Bluetooth 4.0 or later).

### Does it work with multiple desks?

The application connects to one desk at a time. If you have multiple desks, you'll need to:
1. Configure which desk to connect to (by name or address)
2. Reconnect when switching between desks

### Can I use it with the Ikea mobile app simultaneously?

No. Bluetooth LE only allows one active connection at a time. You must disconnect one application before connecting the other.

## Installation Questions

### Do I need to install .NET?

No! The released executable is self-contained and includes all necessary runtime components. Just download and run.

### Why does Windows show a security warning?

Windows SmartScreen shows warnings for downloaded applications that don't have widespread installation history. The application is safe - click "More info" → "Run anyway". The project is open source, so you can review the code yourself for security.

### Where should I install the application?

You can place the executable anywhere convenient:
- Desktop
- Documents folder
- Program Files (requires admin)
- Custom location of your choice

The application stores settings in your user profile, not in its installation directory.

### How do I make it start automatically?

1. Press `Win + R`
2. Type `shell:startup` and press Enter
3. Create a shortcut to the executable in this folder
4. The app will now start when you log in

### Can I install it for all users?

Yes, but each user will have their own settings. Place the executable in a shared location (like Program Files) and have each user create a startup shortcut if desired.

## Usage Questions

### How do I set my preferred desk heights?

Two methods:

**Method 1: Manual Entry**
1. Use desk physical controls to move to desired height
2. Open Settings → General
3. Enter the height value
4. Repeat for each position

**Method 2: Fine-tune in Dialog**
1. Trigger any move command
2. Use arrow keys in confirmation dialog to adjust
3. Height is automatically saved

### Can I have more than 4 preset positions?

Currently, the application supports exactly 4 preset positions:
- Standing
- Seating
- Custom 1
- Custom 2

This covers most use cases, but future versions may add more flexibility.

### What if I only use 2 positions?

You can hide unused positions from the context menu:
1. Settings → General
2. Uncheck "Tray" for positions you don't use
3. Cleaner menu, but hotkeys still work

### How accurate is the positioning?

The desk positioning is very accurate (typically within 1mm) because:
- The desk reports its exact height
- The application calculates stopping distance
- You can fine-tune positions with arrow keys

### Can I control the desk from multiple computers?

Yes, but not simultaneously. You can:
- Pair the desk with multiple Windows computers
- Run the application on each
- Only one can connect at a time
- Disconnect from one before connecting from another

### Does it remember my last position?

The application remembers your configured preset positions. The desk itself remembers its current height (even when powered off), which the application reads when it connects.

## Troubleshooting Questions

### Why can't the app find my desk?

Common causes:
1. **Not paired** - Pair in Windows Bluetooth settings first
2. **Already connected** - Close Ikea mobile app
3. **Out of range** - Move computer closer to desk
4. **Interference** - Check for Bluetooth interference
5. **Wrong name** - Configure desk name in Settings

See [Troubleshooting](Troubleshooting) for detailed solutions.

### Why does connection take so long?

The application searches for Bluetooth devices by name. Speed it up by:
1. Settings → Advanced
2. Enter your desk's Bluetooth MAC address
3. Application can then connect directly

### The desk overshoots my target height. How do I fix it?

Adjust the stopping calculation:
1. Settings → Advanced → Units Till Stop
2. Increase value to stop earlier
3. Test and adjust incrementally

### Hotkeys aren't working. What's wrong?

Check these:
1. **Conflicts** - Another app may use same combination
2. **Registration** - Try restarting application
3. **Configuration** - Verify in Settings → Hot Keys
4. **Focus** - Hotkeys work globally, not just when focused

### Why do my settings reset?

Usually caused by:
1. **Permissions** - Check folder write permissions
2. **Admin mode** - Don't run as administrator unless needed
3. **Disk space** - Ensure adequate free space

## Feature Questions

### Can I create custom hotkeys?

Yes! Settings → Hot Keys allows you to configure any key combination for each position. Use modifiers (Ctrl, Shift, Alt) to avoid conflicts.

### What is Parental Lock?

When enabled (Settings → Advanced), pressing the physical up/down buttons on the desk controller immediately stops movement. This prevents unauthorized adjustments, useful in:
- Homes with children
- Shared office spaces
- Public areas

### Can I change the theme?

Yes! Settings → Appearance offers multiple color themes to match your desktop environment.

### Does it show desktop notifications?

Yes, notifications appear for:
- Connection status changes
- Movement completion
- Errors and warnings

Configure in Windows notification settings if you want to disable them.

### Can I see movement progress?

Yes! The system tray icon updates in real-time showing the current desk height as it moves.

### What's the Stop command for?

The Stop command immediately halts desk movement. Useful for:
- Emergency stops
- Changed your mind mid-movement
- Something in the way

Physical desk buttons also stop movement.

## Technical Questions

### How does it communicate with the desk?

The application uses Bluetooth Low Energy (BLE) to communicate with the desk's control panel. It sends commands and receives status updates using the same protocol as the Ikea mobile app.

### Does it work offline?

The application doesn't require internet connectivity. It only needs:
- Bluetooth connection to desk
- Local Windows installation
- Local settings storage

### Where are my settings stored?

Settings are stored per Windows user in:
```
%LOCALAPPDATA%\IdasenSystemTray\
```

The exact path is shown in Settings → Advanced.

### Where are log files stored?

Logs are stored in:
```
%LOCALAPPDATA%\IdasenSystemTray\Logs\
```

Access via Settings → Advanced → Log Folder.

### Does it collect any data?

No! The application:
- ❌ Doesn't send telemetry
- ❌ Doesn't collect usage data
- ❌ Doesn't connect to external services
- ✅ Only communicates with your desk locally
- ✅ All settings and logs stay on your computer

### Is the code reviewed?

Yes! The project includes:
- **SonarCloud** analysis - Continuous code quality checks
- **CodeQL** scanning - Security vulnerability detection
- **Community review** - Open source for public inspection
- **Unit tests** - Comprehensive test coverage

## Contributing Questions

### How can I contribute?

Several ways to contribute:
1. **Report bugs** - [Open an issue](https://github.com/tschroedter/idasen-desk/issues)
2. **Suggest features** - Use [Discussions](https://github.com/tschroedter/idasen-desk/discussions)
3. **Contribute code** - Submit pull requests
4. **Improve docs** - Documentation improvements welcome
5. **Help others** - Answer questions in Discussions

See [Developer Guide](Developer-Guide) for details.

### Do I need to know C# to contribute?

Not necessarily! You can contribute by:
- Improving documentation
- Creating tutorials
- Testing and reporting bugs
- Translating (if we add internationalization)
- Helping in Discussions

Code contributions do require C# knowledge.

### How do I report a bug?

1. Check if it's already reported in [Issues](https://github.com/tschroedter/idasen-desk/issues)
2. If not, create a new issue with:
   - Detailed description
   - Steps to reproduce
   - Expected vs actual behavior
   - System information
   - Log files if applicable

### Can I request new features?

Absolutely! Use [GitHub Discussions](https://github.com/tschroedter/idasen-desk/discussions) to propose new features. Discuss before implementing to ensure alignment with project goals.

### How long does it take to get help?

This is a community project maintained by volunteers. Response times vary, but:
- Common issues usually answered quickly
- Complex problems may take longer
- Be patient and respectful
- Provide detailed information to get faster help

## License Questions

### Can I use this commercially?

Yes! The MIT License allows commercial use. You can:
- Use it in business environments
- Include it in commercial products
- Modify and distribute

See the [LICENSE](https://github.com/tschroedter/idasen-desk/blob/main/LICENSE) for full details.

### Can I modify the code?

Yes! You can:
- Modify for personal use
- Fork the repository
- Create derivative works
- Submit improvements back to the project

The MIT License is very permissive.

### Do I need to credit the project?

While the MIT License doesn't require attribution for use, it does require the license and copyright notice to be included if you distribute the software or derivatives. Crediting the project is appreciated but not legally required for personal use.

## Still Have Questions?

If your question isn't answered here:

1. 📖 Check the **[User Guide](User-Guide)**
2. 🔧 Review **[Troubleshooting](Troubleshooting)**
3. 🔍 Search [GitHub Issues](https://github.com/tschroedter/idasen-desk/issues)
4. 💬 Ask in [GitHub Discussions](https://github.com/tschroedter/idasen-desk/discussions)
5. 🐛 [Create an issue](https://github.com/tschroedter/idasen-desk/issues/new) if you found a bug

---

**Navigation**: [Home](Home) | [Getting Started](Getting-Started) | [User Guide](User-Guide) | [FAQ](FAQ) | [Troubleshooting](Troubleshooting)
