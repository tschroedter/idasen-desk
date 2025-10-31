# Release Notes

Version history and changelog for the Idasen Desk Controller.

## Overview

This page provides a summary of releases. For detailed changelog entries, see [CHANGELOG.md](https://github.com/tschroedter/idasen-desk/blob/main/CHANGELOG.md).

## Latest Release

### Version 0.1.280

**Release Date:** Latest

**Highlights:**
- 🎨 Enable support for mouse wheel scrolling inside 'SettingsPage'

**Download:** [Latest Release](https://github.com/tschroedter/idasen-desk/releases/latest)

## Recent Releases

### Version 0.1.250

**Focus:** Stability and quality-of-life improvements

**Key Features:**
- 🛡️ **Fixes**: Resolved multiple crashes and edge-case bugs
- 📡 **Improvements**: Reduced memory/CPU usage in core processing
- 📊 **Quality**: SonarCloud scanning for continuous quality checks
- 📦 **Maintenance**: Updated dependencies and security patches

**Improvements:**
- Enhanced logging for easier troubleshooting
- Performance optimizations
- Better error handling
- Security updates

### Version 0.1.182

**Focus:** Enhanced functionality and user experience

**New Features:**
- ✨ **2 New Positions**: Custom 1 and Custom 2 desk positions
- ⚙️ **Fine-tuning**: Use up/down icons or arrow keys in confirmation dialog
- 🎨 **Enhanced UI**: Improved system tray application interface

**Configuration:**
- 👁️ **Visibility Control**: Show/hide positions and Stop command in menu
- 🔌 **Smart Menu**: Shows only Connect or Disconnect based on state

**User Experience:**
- 🖱️ **Click Icon**: Show/hide settings window
- 🪟 **Resizable Settings**: Improved window management
- 📏 **Position Fine-tuning**: Adjust heights with arrow keys

**Technical:**
- 📡 **Bluetooth**: Improved BLE connection handling
- 📦 **Updates**: Latest NuGet packages
- 🔒 **Security**: Masked sensitive data in logs
- 🛡️ **Fixes**: Security vulnerabilities patched
- 📊 **Quality**: SonarCloud integration
- 🐛 **Bug Fixes**: Various code smells addressed

## Version History

For complete version history, see:
- **[CHANGELOG.md](https://github.com/tschroedter/idasen-desk/blob/main/CHANGELOG.md)** - Detailed changelog
- **[GitHub Releases](https://github.com/tschroedter/idasen-desk/releases)** - All releases with assets

## Release Cadence

The project follows **semantic versioning** (SemVer):

- **Major (x.0.0)**: Breaking changes, major features
- **Minor (0.x.0)**: New features, backwards compatible
- **Patch (0.0.x)**: Bug fixes, minor improvements

**Release Frequency:**
- Regular updates as features and fixes are ready
- Security patches released promptly
- Community-driven development

## Upgrade Guide

### Automatic Updates

Currently, updates are manual:
1. Check [Releases page](https://github.com/tschroedter/idasen-desk/releases)
2. Download latest version
3. Exit current application
4. Replace executable
5. Start new version

Settings are preserved automatically (stored separately).

### Using Scoop

If installed via Scoop:

```powershell
scoop update tschroedter/idasen-systemtray
```

### Checking Your Version

To find your current version:
1. Right-click system tray icon
2. Select "About" or check Settings → About
3. Or check executable properties in File Explorer

## Breaking Changes

### Version 0.1.x → 0.2.x (Future)

When version 0.2.0 is released, breaking changes will be documented here.

**Current Status:** No breaking changes in 0.1.x series

## Deprecation Notices

### Current Deprecations

None at this time.

### Future Changes

Stay informed about upcoming changes:
- Watch the [GitHub repository](https://github.com/tschroedter/idasen-desk)
- Check [Discussions](https://github.com/tschroedter/idasen-desk/discussions)
- Review milestone plans

## Getting Notified of New Releases

### GitHub Watch

1. Go to [repository](https://github.com/tschroedter/idasen-desk)
2. Click "Watch" → "Custom"
3. Select "Releases"
4. Click "Apply"

You'll receive notifications for new releases.

### RSS Feed

Subscribe to releases feed:
```
https://github.com/tschroedter/idasen-desk/releases.atom
```

## Reporting Issues

Found a bug in a release?

1. Check [known issues](https://github.com/tschroedter/idasen-desk/issues)
2. Search for similar reports
3. If new, [create an issue](https://github.com/tschroedter/idasen-desk/issues/new)

Include:
- Version number
- Windows version
- Steps to reproduce
- Expected vs actual behavior

## Contributing to Releases

Want to help with the next release?

- **Code**: Submit pull requests
- **Testing**: Test pre-release versions
- **Documentation**: Improve docs
- **Feedback**: Suggest features

See [Developer Guide](Developer-Guide) for details.

## Release Process

Interested in how releases are made?

The project uses automated release management:

1. **Pull Requests**: Labeled with change type
2. **Merge to Main**: Updates draft release
3. **Release Drafter**: Auto-generates release notes
4. **Publish**: Maintainer publishes when ready
5. **Changelog**: Automatically updated
6. **Assets**: Built and attached by CI/CD

Details: [Changelog Automation](https://github.com/tschroedter/idasen-desk/blob/main/docs/CHANGELOG_AUTOMATION.md)

## Download Options

### Direct Download

- [Latest Release](https://github.com/tschroedter/idasen-desk/releases/latest) - Stable
- [All Releases](https://github.com/tschroedter/idasen-desk/releases) - Version history

### Package Manager

```powershell
scoop install tschroedter/idasen-systemtray
```

### Build from Source

See [Build Instructions](Build-Instructions) for compiling yourself.

## Support Lifecycle

**Active Support:**
- Latest minor version receives updates
- Security patches for current version
- Bug fixes for current version

**Community Support:**
- All versions supported by community
- Help available via Discussions
- Issues tracked on GitHub

## Statistics

View project activity:
- **[Contributors](https://github.com/tschroedter/idasen-desk/graphs/contributors)** - Who's contributing
- **[Commit Activity](https://github.com/tschroedter/idasen-desk/graphs/commit-activity)** - Development pace
- **[Code Frequency](https://github.com/tschroedter/idasen-desk/graphs/code-frequency)** - Additions/deletions

---

**Navigation**: [Home](Home) | [Getting Started](Getting-Started) | [Release Notes](Release-Notes) | [FAQ](FAQ)
