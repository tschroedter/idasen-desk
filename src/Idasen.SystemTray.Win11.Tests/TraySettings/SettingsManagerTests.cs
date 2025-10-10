using System.IO.Abstractions ;
using FluentAssertions ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;
using NSubstitute ;
using NSubstitute.ExceptionExtensions ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Tests.TraySettings ;

public class SettingsManagerTests
    : IDisposable
{
    private readonly ICommonApplicationData _commonApplicationData = Substitute.For < ICommonApplicationData > ( ) ;
    private readonly IFileSystem            _fileSystem            = Substitute.For < IFileSystem > ( ) ;
    private readonly ILogger                _logger                = Substitute.For < ILogger > ( ) ;
    private readonly SettingsManager        _settingsManager ;
    private readonly ISettingsStorage       _settingsStorage = Substitute.For < ISettingsStorage > ( ) ;

    public SettingsManagerTests ( )
    {
        _commonApplicationData.ToFullPath ( Constants.SettingsFileName ).Returns ( "TestSettingsFilePath" ) ;
        _settingsManager = new SettingsManager ( _logger ,
                                                 _commonApplicationData ,
                                                 _settingsStorage ,
                                                 _fileSystem ) ;
    }

    public void Dispose ( )
    {
        _settingsManager.Dispose ( ) ;

        GC.SuppressFinalize ( this ) ;
    }

    [ Fact ]
    public async Task SaveAsync_ShouldSaveSettings ( )
    {
        // Arrange
        _settingsManager.CurrentSettings.DeviceSettings.DeviceName = "TestDevice" ;

        // Act
        await _settingsManager.SaveAsync ( CancellationToken.None ) ;

        // Assert
        await _settingsStorage.Received ( 1 ).SaveSettingsAsync ( "TestSettingsFilePath" ,
                                                                  Arg.Is < Settings > ( s =>
                                                                                            s.DeviceSettings
                                                                                             .DeviceName ==
                                                                                            "TestDevice" ) ,
                                                                  CancellationToken.None ) ;
    }

    [ Fact ]
    public async Task LoadAsync_ShouldLoadSettings ( )
    {
        // Arrange
        var settings = new Settings { DeviceSettings = { DeviceName = "LoadedDevice" } } ;
        _settingsStorage.LoadSettingsAsync ( "TestSettingsFilePath" ,
                                             CancellationToken.None ).Returns ( settings ) ;

        // Act
        await _settingsManager.LoadAsync ( CancellationToken.None ) ;

        // Assert
        _settingsManager.CurrentSettings
                        .DeviceSettings
                        .DeviceName
                        .Should ( )
                        .Be ( "LoadedDevice" ) ;
    }

    [ Fact ]
    public async Task UpgradeSettingsAsync_ShouldAddMissingSettings ( )
    {
        // Arrange
        await File.WriteAllTextAsync ( "TestSettingsFilePath" ,
                                       "{}" ) ;
        _fileSystem.File.Exists ( "TestSettingsFilePath" ).Returns ( true ) ;
        _fileSystem.File.ReadAllTextAsync ( "TestSettingsFilePath" ,
                                            CancellationToken.None ).Returns ( "{}" ) ;
        _settingsStorage.LoadSettingsAsync ( "TestSettingsFilePath" ,
                                             CancellationToken.None ).Returns ( new Settings ( ) ) ;

        // Act
        var result = await _settingsManager.UpgradeSettingsAsync ( CancellationToken.None ) ;

        // Assert
        result.Should ( ).BeTrue ( ) ;
        _settingsManager.CurrentSettings
                        .DeviceSettings
                        .NotificationsEnabled
                        .Should ( )
                        .BeTrue ( ) ;
    }

    [ Fact ]
    public async Task SetLastKnownDeskHeight_ShouldUpdateHeightAndSave ( )
    {
        // Arrange
        const uint heightInCm = 120 ;

        // Act
        await _settingsManager.SetLastKnownDeskHeight ( heightInCm ,
                                                        CancellationToken.None ) ;

        // Assert
        _settingsManager.CurrentSettings
                        .HeightSettings
                        .LastKnownDeskHeight
                        .Should ( )
                        .Be ( heightInCm ) ;

        await _settingsStorage.Received ( 1 )
                              .SaveSettingsAsync ( "TestSettingsFilePath" ,
                                                   Arg.Is < Settings > ( s => s.HeightSettings.LastKnownDeskHeight ==
                                                                              heightInCm ) ,
                                                   CancellationToken.None ) ;
    }

    [ Fact ]
    public async Task ResetSettingsAsync_ShouldCreateDefaults_Save_AndNotify ( )
    {
        // Arrange
        var notified = false ;
        _settingsManager.SettingsSaved.Subscribe ( _ => notified = true ) ;

        _settingsManager.CurrentSettings.DeviceSettings.DeviceName = "Changed" ;

        // Act
        await _settingsManager.ResetSettingsAsync ( CancellationToken.None ) ;

        // Assert
        await _settingsStorage.Received ( 1 )
                              .SaveSettingsAsync ( "TestSettingsFilePath" ,
                                                   Arg.Is < Settings > ( s => s.DeviceSettings.DeviceName ==
                                                                              Constants.DefaultDeviceName &&
                                                                              s.HeightSettings.StandingHeightInCm ==
                                                                              Constants.DefaultHeightStandingInCm ) ,
                                                   CancellationToken.None ) ;
        notified.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public async Task ResetSettingsAsync_ShouldLogErrorAndThrowInvalidOperationException_WhenSaveFails ( )
    {
        // Arrange
        var testException = new ArgumentException ( "Save failed" ) ;
        _settingsStorage
           .SaveSettingsAsync ( Arg.Any < string > ( ) ,
                                Arg.Any < Settings > ( ) ,
                                Arg.Any < CancellationToken > ( ) )
           .Throws ( testException ) ;

        // Act
        var act = async ( ) => await _settingsManager.ResetSettingsAsync ( CancellationToken.None ) ;

        // Assert
        await act.Should ( ).ThrowAsync < InvalidOperationException > ( )
                 .WithMessage ( "Failed to reset settings" ) ;
        _logger.Received ( 1 ).Error ( testException ,
                                       "Failed to reset settings" ) ;
    }
}