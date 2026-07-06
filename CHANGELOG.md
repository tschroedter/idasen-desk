# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased](https://github.com/tschroedter/idasen-desk/compare/v0.1.492...HEAD)

## [0.1.78](https://github.com/tschroedter/idasen-desk/releases/tag/V0.1.78) - 2024-10-07

### Added

- Updated .NET 4.8 to .NET 8.0
- Updated used NuGet Packages to latest
- Updated UI to look more like native Windows application
- Updated notification to use Windows build in notifications
- Added support for themes
- Remember last desk height

### Fixed

- Fixed vulnerabilities in external NuGet packages by updating to latest version

## [v0.1.492](https://github.com/tschroedter/idasen-desk/compare/v0.1.484...v0.1.492) - 2026-07-06

Changes

- Add comprehensive height settings validation
- Allow renaming custom desk position presets
- Updated NuGet packages

## [v0.1.484](https://github.com/tschroedter/idasen-desk/compare/v0.1.443...v0.1.484) - 2026-07-01

Changes:

- Allow renaming of desk position preset
- Improved Bluetooth reliability with stale connection detection and automatic reconnection.
- Refactored desk orchestration into focused managers (`DeskConnectionManager`, `DeskMovementManager`, `DeskNotificationManager`, `DeskReadyManager`, `StatusBarManager`) with expanded test coverage.

## [v0.1.443](https://github.com/tschroedter/idasen-desk/compare/v0.1.435...v0.1.443) - 2026-05-22

Global Hotkey Improvements

## [v0.1.435](https://github.com/tschroedter/idasen-desk/compare/v0.1.420...v0.1.435) - 2026-05-22

🚀 Features

- Configurable global hotkeys (UI-based)
- Enable/disable hotkeys (UI-based)

## [v0.1.420](https://github.com/tschroedter/idasen-desk/compare/v0.1.401...v0.1.420) - 2026-05-17

### Changes

### 🚀 Features

- Option to disable and customise global hotkeys
  - Added configurable global hotkeys in Settings
  - Added support for enabling/disabling global hotkeys
  - Added selectable hotkey keys and modifier combinations
  - Added available key provider and hotkey definition UI controls
  - Added hotkey settings persistence and synchronisation
  

### 🐛 Fixes

- Global Hotkey Improvements: Fix Startup Registration and Enhance UX
  - Fixed startup hotkey registration timing so persisted settings are respected
  - Improved hotkey registration/un-registration behaviour when settings change
  - Added safer hotkey replacement and unregister logic
  - Improved logging around hotkey lifecycle events
  

### 📝 Documentation

- Added hotkey customisation guide
  - New `HOTKEY_CUSTOMIZATION.md` explaining how to disable and customise hotkeys
  - Included examples for F-keys, arrow keys, numpad keys, and modifiers
  

### ✅ Tests

- Added comprehensive test coverage for hotkey settings, parsing, synchronisation, storage, and UI behaviour

## [v0.1.401](https://github.com/tschroedter/idasen-desk/compare/v0.1.387...v0.1.401) - 2026-05-06

Fix the donation link.

## [v0.1.387](https://github.com/tschroedter/idasen-desk/compare/v0.1.357...v0.1.387) - 2026-05-02

### Changes

- Upgrade to .NET 10.0 and resolve security vulnerability @tschroedter (https://github.com/tschroedter/idasen-desk/pull/106)
- Update NuGet packages to latest compatible versions @tschroedter (https://github.com/tschroedter/idasen-desk/pull/105)
- chore: update all NuGet packages to latest versions @tschroedter (https://github.com/tschroedter/idasen-desk/pull/104)
- Skip SonarCloud analysis for fork PRs to prevent secret access failures @[copilot-swe-agent[bot]](https://github.com/apps/copilot-swe-agent) (https://github.com/tschroedter/idasen-desk/pull/100)

### 🐛 Bug Fixes

- 108 desk cannot be controlled after interrupted desk movement @tschroedter (https://github.com/tschroedter/idasen-desk/pull/109)
- fix: increase test delays for CI reliability @tschroedter (https://github.com/tschroedter/idasen-desk/pull/107)

### 🧰 Maintenance

- chore(deps): bump release-drafter/release-drafter from 6 to 7 @[dependabot[bot]](https://github.com/apps/dependabot) (https://github.com/tschroedter/idasen-desk/pull/102)
- chore(deps): bump actions/cache from 4 to 5 @[dependabot[bot]](https://github.com/apps/dependabot) (https://github.com/tschroedter/idasen-desk/pull/99)

## [v0.1.357](https://github.com/tschroedter/idasen-desk/compare/v0.1.350...v0.1.357) - 2025-12-10

Updated NuGet packages.

## [v0.1.350](https://github.com/tschroedter/idasen-desk/compare/v0.1.345...v0.1.350) - 2025-12-08

Fix minor SonarCloud issue: '...class should be marked partial...'

## [v0.1.345](https://github.com/tschroedter/idasen-desk/compare/v0.1.336...v0.1.345) - 2025-12-04

Minor maintenance release with bug fixes to catch and handle rare COMException: (0x80263001): {Desktop composition is disabled}

## [v0.1.336](https://github.com/tschroedter/idasen-desk/compare/v0.1.329...v0.1.336) - 2025-11-12

Fixes

- Restore theme after resuming from hibernation.
- Restore theme when connecting eGPU.

## [v0.1.329](https://github.com/tschroedter/idasen-desk/compare/v0.1.299...v0.1.329) - 2025-11-09

Fix failing to load settings and applying settings.

## [v0.1.299](https://github.com/tschroedter/idasen-desk/compare/v0.1.280...v0.1.299) - 2025-11-01

Changes:

- Increased scrolling speed for mouse wheel scrolling over expander controls.

## [v0.1.280](https://github.com/tschroedter/idasen-desk/compare/v0.1.250...v0.1.280) - 2025-10-30

Changes:

- Enable mouse wheel scrolling on 'SettingsPage'

## [v0.1.250](https://github.com/tschroedter/idasen-desk/compare/v0.1.182...v0.1.250) - 2025-10-27

Short description: This patch release focuses on stability and quality-of-life improvements. It includes several bug fixes, performance optimizations, small usability improvements, dependency/security updates, and adds SonarCloud scanning so all code is now continuously checked for quality and security.

Highlights:

- Fixes: resolved multiple reported crashes and edge-case bugs.
- Improvements: reduced memory/CPU usage in core processing paths and improved logging for easier troubleshooting.
- Quality: all code is now scanned by SonarCloud (static analysis for bugs, code smells, and security issues).
- Maintenance: updated third-party dependencies and applied security patches.

## [v0.1.182](https://github.com/tschroedter/idasen-desk/compare/v0.1.181...v0.1.182) - 2025-10-13

Test Release for scoop.sh testing.

### Changes

* No changes

## [v0.1.181](https://github.com/tschroedter/idasen-desk/compare/v0.1.175...v0.1.181) - 2025-10-13

Fix typo in confirmation dialog.

## [v0.1.175](https://github.com/tschroedter/idasen-desk/compare/V0.1.171...v0.1.175) - 2025-10-12

Test Release for scoop.sh setup. - No Code Changes!

## [V0.1.171](https://github.com/tschroedter/idasen-desk/compare/V0.1.78...V0.1.171) - 2025-10-12

Minor update of the README.MD file.
