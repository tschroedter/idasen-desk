# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased](https://github.com/tschroedter/idasen-desk/compare/v0.1.350...HEAD)

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
