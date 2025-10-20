using System.IO.Abstractions.TestingHelpers ;
using System.Text.Json ;
using FluentAssertions ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.Tests.TraySettings ;

public class SettingsStorageTests
{
    private const string TestFileName = "/temp/testSettings.json" ;

    private readonly MockFileSystem _mockFileSystem = new( ) ;

    private readonly Settings _settings = new( )
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

    // New tests to increase coverage

    [ Fact ]
    public async Task LoadSettingsAsync_FileMissing_SetsThemeFromWindowsDefaults ( )
    {
        // Act
        var result = await CreateSut ( ).LoadSettingsAsync ( "/does/not/exist/settings.json" ,
                                                             CancellationToken.None ) ;

        // Assert
        result.AppearanceSettings.ThemeName.Should ( ).Be ( ThemeDefaults.GetDefaultThemeName ( ) ) ;
    }

    [ Fact ]
    public async Task LoadSettingsAsync_InvalidJson_ThrowsIOException ( )
    {
        // Arrange
        var path = "/temp/bad.json" ;
        _mockFileSystem.AddFile ( path ,
                                  new MockFileData ( "this is not valid json" ) ) ;

        // Act
        Func < Task > act = async ( ) => await CreateSut ( ).LoadSettingsAsync ( path ,
                                                                                 CancellationToken.None ) ;

        // Assert
        await act.Should ( ).ThrowAsync < IOException > ( )
                 .WithMessage ( $"Failed to load settings from {path}" ) ;
    }

    [ Fact ]
    public async Task LoadSettingsAsync_NullJson_ReturnsNewSettings ( )
    {
        // Arrange
        var path = "/temp/null.json" ;
        _mockFileSystem.AddFile ( path ,
                                  new MockFileData ( "null" ) ) ;

        // Act
        var result = await CreateSut ( ).LoadSettingsAsync ( path ,
                                                             CancellationToken.None ) ;

        // Assert
        result.Should ( ).NotBeNull ( ) ;
        result.DeviceSettings.Should ( ).NotBeNull ( ) ;
        result.HeightSettings.Should ( ).NotBeNull ( ) ;
        result.AppearanceSettings.Should ( ).NotBeNull ( ) ;
    }

    [ Fact ]
    public async Task SaveSettingsAsync_CreatesDirectoryIfMissing ( )
    {
        // Arrange
        var mfs  = new MockFileSystem ( ) ;
        var sut  = new SettingsStorage ( mfs ) ;
        var path = "/new/dir/structure/settings.json" ;

        // Act
        await sut.SaveSettingsAsync ( path ,
                                      _settings ,
                                      CancellationToken.None ) ;

        // Assert
        mfs.Directory.Exists ( "/new/dir/structure" ).Should ( ).BeTrue ( ) ;
        mfs.File.Exists ( path ).Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public async Task SaveSettingsAsync_CanceledToken_ThrowsOperationCanceledException ( )
    {
        // Arrange
        using var cts = new CancellationTokenSource ( ) ;

        await cts.CancelAsync ( ) ;

        // Act
        // ReSharper disable AccessToDisposedClosure
        var act = async ( ) => await CreateSut ( ).SaveSettingsAsync ( TestFileName ,
                                                                       _settings ,
                                                                       cts.Token ) ;
        // ReSharper restore AccessToDisposedClosure

        // Assert
        await act.Should ( ).ThrowAsync < OperationCanceledException > ( ) ;
    }

    private SettingsStorage CreateSut ( )
    {
        _mockFileSystem.AddDirectory ( Path.GetDirectoryName ( TestFileName ) ?? string.Empty ) ;

        return new SettingsStorage ( _mockFileSystem ) ;
    }
}