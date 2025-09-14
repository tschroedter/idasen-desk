using System.IO.Abstractions.TestingHelpers ;
using System.Text.Json ;
using FluentAssertions ;
using Idasen.SystemTray.Win11.TraySettings ;

namespace Idasen.SystemTray.Win11.Tests.TraySettings ;

public class SettingsStorageTests
{
    private const string TestFileName = "/temp/testSettings.json" ;

    private readonly MockFileSystem _mockFileSystem = new ( ) ;

    private readonly Settings _settings = new ( )
    {
        DeviceSettings = new DeviceSettings { DeviceName         = "TestDevice" } ,
        HeightSettings = new HeightSettings { StandingHeightInCm = 120 }
    } ;

    [ Fact ]
    public async Task LoadSettingsAsync_FileDoesNotExist_ReturnsDefaultSettings ( )
    {
        // Arrange

        // Act
        var result = await CreateSut ( ).LoadSettingsAsync ( TestFileName ,
                                                             CancellationToken.None ) ;

        // Assert
        result.Should ( ).NotBeNull ( ) ;
        result.Should ( ).BeOfType < Settings > ( ) ;
    }

    [ Fact ]
    public async Task LoadSettingsAsync_FileExists_ReturnsDeserializedSettings ( )
    {
        // Arrange
        var serialize = JsonSerializer.Serialize ( _settings ,
                                                   SettingsStorage.JsonOptions ) ;
        _mockFileSystem.AddFile ( TestFileName ,
                                  new MockFileData ( serialize ) ) ;

        // Act
        var result = await CreateSut ( ).LoadSettingsAsync ( TestFileName ,
                                                             CancellationToken.None ) ;

        // Assert
        result.Should ( ).NotBeNull ( ) ;
        result.DeviceSettings
              .DeviceName
              .Should ( )
              .Be ( _settings.DeviceSettings.DeviceName ) ;
        result.HeightSettings
              .StandingHeightInCm
              .Should ( )
              .Be ( _settings.HeightSettings.StandingHeightInCm ) ;
    }

    [ Fact ]
    public async Task SaveSettingsAsync_ValidSettings_SavesToFile ( )
    {
        // Arrange

        // Act
        await CreateSut ( ).SaveSettingsAsync ( TestFileName ,
                                                _settings ,
                                                CancellationToken.None ) ;

        // Assert
        _mockFileSystem.File
                       .Exists ( TestFileName )
                       .Should ( )
                       .BeTrue ( ) ;
        _mockFileSystem.FileInfo
                       .New ( TestFileName ).Length
                       .Should ( )
                       .BeGreaterThan ( 0 ) ;
    }

    [ Fact ]
    public async Task SaveSettingsAsync_InvalidFileName_ThrowsIOException ( )
    {
        // Arrange
        var settings        = new Settings ( ) ;
        var invalidFileName = string.Empty ;

        // Act & Assert
        await FluentActions.Invoking ( ( ) => CreateSut ( ).SaveSettingsAsync ( invalidFileName ,
                                                                                settings ,
                                                                                CancellationToken.None ) )
                           .Should ( )
                           .ThrowAsync < IOException > ( ) ;
    }

    private SettingsStorage CreateSut ( )
    {
        _mockFileSystem.AddDirectory ( Path.GetDirectoryName ( TestFileName ) ?? string.Empty ) ;

        return new SettingsStorage ( _mockFileSystem ) ;
    }
}