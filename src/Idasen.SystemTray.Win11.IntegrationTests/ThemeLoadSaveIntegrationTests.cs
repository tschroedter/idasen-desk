using System;
using System.IO.Abstractions.TestingHelpers;
using System.IO.Abstractions;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading ;
using System.Threading.Tasks;
using Idasen.SystemTray.Win11.TraySettings;
using Idasen.SystemTray.Win11.Utils;
using NSubstitute;
using Serilog;
using Wpf.Ui.Appearance ;
using Xunit;

namespace Idasen.SystemTray.Win11.IntegrationTests;

public class ThemeLoadSaveIntegrationTests
{
    [Fact]
    public async Task SettingsManager_LoadSave_WithThemeApply_DoesNotThrow()
    {
        // Arrange - use real SettingsStorage with MockFileSystem
        var fs = new MockFileSystem();
        var path = "/temp/testsettings.json";

        var defaults = new Settings();
        defaults.AppearanceSettings.ThemeName = "Light";
        var content = JsonSerializer.Serialize(defaults, SettingsStorage.JsonOptions);
        fs.AddFile(path, new MockFileData(content));

        var storage = new SettingsStorage(fs);

        // Provide a common application data that returns our test path
        var commonAppData = Substitute.For<Idasen.SystemTray.Win11.Interfaces.ICommonApplicationData>();
        commonAppData.ToFullPath(Arg.Any<string>()).Returns(path);

        var logger = Substitute.For<ILogger>();

        var settingsManager = new SettingsManager(logger, commonAppData, storage, fs);

        // Act & Assert - ensure load/save cycles don't throw under theme apply
        await settingsManager.LoadAsync(CancellationToken.None);

        var themeManager = new MyApplicationThemeManager(logger);
        await themeManager.ApplyAsync(ApplicationTheme.Light); // should not throw

        await settingsManager.SaveAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Stress_LoadSave_ConcurrentSaves_DoesNotThrow()
    {
        // Use a real filesystem on a temp folder to exercise real locks
        var tempDir = Path.Combine(Path.GetTempPath(), "idasen_test_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);

        var settingsPath = Path.Combine(tempDir, "settings.json");

        var defaults = new Settings();
        defaults.AppearanceSettings.ThemeName = "Light";
        await File.WriteAllTextAsync(settingsPath, JsonSerializer.Serialize(defaults, SettingsStorage.JsonOptions));

        var fs = new FileSystem();
        var storage = new SettingsStorage(fs);

        var commonAppData = Substitute.For<Idasen.SystemTray.Win11.Interfaces.ICommonApplicationData>();
        commonAppData.ToFullPath(Arg.Any<string>()).Returns(settingsPath);

        var logger = Substitute.For<ILogger>();

        var settingsManager = new SettingsManager(logger, commonAppData, storage, fs);
        var themeManager = new MyApplicationThemeManager(logger);

        var cts = CancellationToken.None;

        const int iterations = 100;
        for (var i = 0; i < iterations; i++)
        {
            await settingsManager.LoadAsync(cts);

            // toggle theme
            var theme = (i % 2 == 0) ? ApplicationTheme.Dark : ApplicationTheme.Light;
            await themeManager.ApplyAsync(theme);

            // Run several concurrent saves to try and provoke sharing/lock behavior
            var tasks = Enumerable.Range(0, 4)
                                  .Select(_ => Task.Run(() => settingsManager.SaveAsync(cts)))
                                  .ToArray();

            await Task.WhenAll(tasks);
        }

        // Clean up
        try
        {
            Directory.Delete(tempDir, true);
        }
        catch
        {
            // best-effort cleanup
        }
    }

    [Fact]
    public async Task DiskBacked_LoadSave_PersistsAcrossManagerInstances()
    {
        // Arrange: create a temp settings file on disk
        var tempDir = Path.Combine(Path.GetTempPath(), "idasen_disk_test_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);

        var settingsPath = Path.Combine(tempDir, "settings.json");

        var defaults = new Settings();
        defaults.AppearanceSettings.ThemeName = "Light";
        await File.WriteAllTextAsync(settingsPath, JsonSerializer.Serialize(defaults, SettingsStorage.JsonOptions));

        var fs = new FileSystem();
        var storage = new SettingsStorage(fs);

        var commonAppData = Substitute.For<Idasen.SystemTray.Win11.Interfaces.ICommonApplicationData>();
        commonAppData.ToFullPath(Arg.Any<string>()).Returns(settingsPath);

        var logger = Substitute.For<ILogger>();

        // Act: create a manager, load, change a setting, save, then create a fresh manager and load again
        var manager1 = new SettingsManager(logger, commonAppData, storage, fs);
        await manager1.LoadAsync(CancellationToken.None);

        // modify and save
        manager1.CurrentSettings.DeviceSettings.DeviceName = "DiskBackedTestDevice";
        await manager1.SaveAsync(CancellationToken.None);

        // Create a new manager instance to simulate a restart
        var manager2 = new SettingsManager(logger, commonAppData, storage, fs);
        await manager2.LoadAsync(CancellationToken.None);

        // Assert: the change persisted to disk and was read by the new instance
        Assert.Equal("DiskBackedTestDevice", manager2.CurrentSettings.DeviceSettings.DeviceName);

        // Clean up
        try
        {
            Directory.Delete(tempDir, true);
        }
        catch
        {
            // best-effort cleanup
        }
    }
}
