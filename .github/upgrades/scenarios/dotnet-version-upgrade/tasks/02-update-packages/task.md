# 02-update-packages: Update all NuGet packages

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
