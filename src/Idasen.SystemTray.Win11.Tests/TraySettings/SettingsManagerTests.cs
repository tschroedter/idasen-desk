using FluentAssertions ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;
using NSubstitute ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Tests.TraySettings ;

public class SettingsManagerTests
{
    private readonly ICommonApplicationData _commonApplicationData = Substitute.For<ICommonApplicationData>();
    private readonly ILogger _logger = Substitute.For<ILogger>();
    private readonly SettingsManager _settingsManager;
    private readonly ISettingsStorage _settingsStorage = Substitute.For<ISettingsStorage>();

    public SettingsManagerTests()
    {
        _commonApplicationData.ToFullPath(Constants.SettingsFileName).Returns("TestSettingsFilePath");
        _settingsManager = new SettingsManager(_logger,
                                                 _commonApplicationData,
                                                 _settingsStorage);
    }

    [Fact]
    public async Task SaveAsync_ShouldSaveSettings()
    {
        // Arrange
        _settingsManager.CurrentSettings.DeviceSettings.DeviceName = "TestDevice";

        // Act
        await _settingsManager.SaveAsync();

        // Assert
        await _settingsStorage.Received(1).SaveSettingsAsync("TestSettingsFilePath",
                                                                  Arg.Is<Settings>(s => s.DeviceSettings.DeviceName == "TestDevice"));
    }

    [Fact]
    public async Task LoadAsync_ShouldLoadSettings()
    {
        // Arrange
        var settings = new Settings { DeviceSettings = { DeviceName = "LoadedDevice" } };
        _settingsStorage.LoadSettingsAsync("TestSettingsFilePath").Returns(settings);

        // Act
        await _settingsManager.LoadAsync();

        // Assert
        _settingsManager.CurrentSettings
                        .DeviceSettings
                        .DeviceName
                        .Should()
                        .Be("LoadedDevice");
    }

    [Fact]
    public async Task UpgradeSettingsAsync_ShouldAddMissingSettings()
    {
        // Arrange
        await File.WriteAllTextAsync("TestSettingsFilePath",
                                       "{}");
        _settingsStorage.LoadSettingsAsync("TestSettingsFilePath").Returns(new Settings());

        // Act
        var result = await _settingsManager.UpgradeSettingsAsync();

        // Assert
        result.Should().BeFalse();
        _settingsManager.CurrentSettings
                        .DeviceSettings
                        .NotificationsEnabled
                        .Should()
                        .BeTrue();
    }

    [Fact]
    public async Task SetLastKnownDeskHeight_ShouldUpdateHeightAndSave()
    {
        // Arrange
        const uint heightInCm = 120;

        // Act
        await _settingsManager.SetLastKnownDeskHeight(heightInCm);

        // Assert
        _settingsManager.CurrentSettings
                        .HeightSettings
                        .LastKnowDeskHeight
                        .Should()
                        .Be(heightInCm);

        await _settingsStorage.Received ( 1 )
                              .SaveSettingsAsync ( "TestSettingsFilePath" ,
                                                   Arg.Is < Settings > ( s => s.HeightSettings.LastKnowDeskHeight == heightInCm ) ) ;
    }
}