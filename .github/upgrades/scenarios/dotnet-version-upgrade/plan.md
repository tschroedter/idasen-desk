# .NET 10.0 Package Update Plan

## Overview

**Target**: Update NuGet packages to latest compatible versions for .NET 10.0
**Scope**: 2 projects (~8.4k LOC, 30 packages)

### Selected Strategy
**All-At-Once** — All packages updated simultaneously in a single operation.
**Rationale**: 2 projects, both already on .NET 10.0-windows10.0.19041, clear dependency structure, all packages compatible.

## Tasks

### 01-prerequisites: Verify SDK and toolchain compatibility

Confirm that the .NET 10.0 SDK is installed and accessible, and validate that any global.json files in the solution are compatible with .NET 10.0. This ensures the build environment is ready for the package updates.

**Done when**: .NET 10.0 SDK verified via `dotnet --list-sdks`, global.json validated (if present), and build environment confirmed ready.

---

### 02-update-packages: Update all NuGet packages

Update all 30 NuGet packages across both projects (Idasen.SystemTray.Win11 and Idasen.SystemTray.Win11.Tests) to their latest compatible versions. The assessment shows all packages are compatible with .NET 10.0, including:

Key packages to update:
- Autofac ecosystem (Autofac, Autofac.Extensions.DependencyInjection, AutofacSerilogIntegration)
- Microsoft.Extensions.Hosting (currently 10.0.7)
- Serilog ecosystem (8 packages)
- Testing packages (xunit, FluentAssertions, NSubstitute, Microsoft.NET.Test.Sdk)
- UI packages (WPF-UI, CommunityToolkit.Mvvm)
- System.IO.Abstractions packages

Review package update suggestions, apply updates across both projects, restore dependencies, and handle any package-specific upgrade notes (API changes, configuration updates, breaking changes in package documentation).

**Done when**: All package references updated to latest compatible versions, dependencies restored successfully, and any package-specific migration steps completed.

---

### 03-build-and-fix: Build solution and resolve any issues

Build the entire solution after package updates and fix any compilation errors or warnings that arise from package API changes or behavioral differences. Address any deprecated API usage flagged by updated packages.

**Done when**: Solution builds with 0 errors and 0 warnings in both Debug and Release configurations.

---

### 04-run-tests: Execute test suite

Run the full test suite in Idasen.SystemTray.Win11.Tests to verify that package updates haven't introduced behavioral regressions. Address any test failures related to package updates (changed behavior, updated test APIs, new package requirements).

**Done when**: All tests pass, test coverage maintained, and any test-specific package updates validated.

---

### 05-final-validation: Validate solution health

Perform final validation of the upgraded solution: verify runtime behavior with updated packages, confirm no dependency conflicts, review any package security advisories addressed by updates, and document any deferred recommendations or optional improvements.

**Done when**: Solution builds cleanly, tests pass, runtime validation complete, and upgrade summary documented.
