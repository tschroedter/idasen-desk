using FluentAssertions ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.TraySettings ;
using NSubstitute ;
using NSubstitute.ExceptionExtensions ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Tests.TraySettings ;

public class LoggingSettingsManagerTests
{
    private readonly ILogger                _logger ;
    private readonly LoggingSettingsManager _manager ;
    private readonly ISettingsManager       _settingsManager ;

    public LoggingSettingsManagerTests ( )
    {
        _logger          = Substitute.For < ILogger > ( ) ;
        _settingsManager = Substitute.For < ISettingsManager > ( ) ;
        _manager = new LoggingSettingsManager ( _logger ,
                                                _settingsManager ) ;
    }

    [ Fact ]
    public async Task SaveAsync_ShouldLogDebugAndCallSaveAsync ( )
    {
        await _manager.SaveAsync ( CancellationToken.None ) ;

        var debugCalls = _logger.ReceivedCalls ( )
                                .Where ( c => c.GetMethodInfo ( ).Name == nameof ( _logger.Debug ) ) ;

        debugCalls.Count ( ).Should ( ).BeGreaterThanOrEqualTo ( 1 ) ;

        await _settingsManager.Received ( 1 )
                              .SaveAsync ( CancellationToken.None ) ;
    }

    [ Fact ]
    public async Task LoadAsync_ShouldLogDebugAndCallLoadAsync ( )
    {
        await _manager.LoadAsync ( CancellationToken.None ) ;

        var debugCalls = _logger.ReceivedCalls ( )
                                .Where ( c => c.GetMethodInfo ( ).Name == nameof ( _logger.Debug ) ) ;

        debugCalls.Count ( ).Should ( ).BeGreaterThanOrEqualTo ( 1 ) ;

        await _settingsManager.Received ( 1 )
                              .LoadAsync ( CancellationToken.None ) ;
    }

    [ Fact ]
    public async Task UpgradeSettingsAsync_ShouldLogDebugAndReturnSuccess ( )
    {
        _settingsManager.UpgradeSettingsAsync ( CancellationToken.None ).Returns ( true ) ;

        var result = await _manager.UpgradeSettingsAsync ( CancellationToken.None ) ;

        result.Should ( )
              .BeTrue ( ) ;

        var debugCalls = _logger.ReceivedCalls ( )
                                .Where ( c => c.GetMethodInfo ( ).Name == nameof ( _logger.Debug ) ) ;

        debugCalls.Count ( ).Should ( ).BeGreaterThanOrEqualTo ( 1 ) ;
    }

    [ Fact ]
    public async Task UpgradeSettingsAsync_ShouldLogErrorAndReturnFalseOnFailure ( )
    {
        _settingsManager.UpgradeSettingsAsync ( CancellationToken.None ).Returns ( false ) ;

        var result = await _manager.UpgradeSettingsAsync ( CancellationToken.None ) ;

        result.Should ( ).BeFalse ( ) ;

        var errorCalls = _logger.ReceivedCalls ( )
                                .Where ( c => c.GetMethodInfo ( ).Name == nameof ( _logger.Error ) ) ;

        errorCalls.Count ( ).Should ( ).BeGreaterThanOrEqualTo ( 1 ) ;
    }

    [ Fact ]
    public async Task SetLastKnownDeskHeight_ShouldLogDebugAndCallSetLastKnownDeskHeight ( )
    {
        const uint heightInCm = 100 ;

        await _manager.SetLastKnownDeskHeight ( heightInCm ,
                                                CancellationToken.None ) ;

        var debugCalls = _logger.ReceivedCalls ( )
                                .Where ( c => c.GetMethodInfo ( ).Name == nameof ( _logger.Debug ) ) ;

        debugCalls.Count ( ).Should ( ).BeGreaterThanOrEqualTo ( 1 ) ;

        await _settingsManager.Received ( 1 )
                              .SetLastKnownDeskHeight ( heightInCm ,
                                                        CancellationToken.None ) ;
    }

    [ Fact ]
    public async Task SaveAsync_ShouldLogErrorAndThrowInvalidOperationException_WhenSaveFails ( )
    {
        // Arrange
        var exception = new ArgumentException ( "Test exception" ) ;

        _settingsManager.SaveAsync ( Arg.Any < CancellationToken > ( ) ).Throws ( exception ) ;

        // Act
        var act = async ( ) => await _manager.SaveAsync ( CancellationToken.None ) ;

        // Assert
        await act.Should ( ).ThrowAsync < InvalidOperationException > ( )
                 .WithMessage ( $"Failed to save settings in file {_settingsManager.SettingsFileName}" ) ;

        _logger.Received ( 1 ).Error ( exception ,
                                       "Failed to save settings in file {SettingsFileName}" ,
                                       _settingsManager.SettingsFileName ) ;
    }

    [ Fact ]
    public async Task LoadAsync_WhenExceptionThrown_ShouldLogErrorAndThrowInvalidOperationException ( )
    {
        // Arrange
        var token = CancellationToken.None ;

        _settingsManager.LoadAsync ( token ).Throws ( new ArgumentException ( "Test exception" ) ) ;

        // Act
        var act = async ( ) => await _manager.LoadAsync ( token ) ;

        // Assert
        await act.Should ( ).ThrowAsync < InvalidOperationException > ( )
                 .WithMessage ( $"Failed to load settings from file {_settingsManager.SettingsFileName}" ) ;
        _logger.Received ( 1 ).Error ( Arg.Any < Exception > ( ) ,
                                       "Failed to load settings" ) ;
    }

    [Fact]
    public void Properties_ShouldReturnValuesFromSettingsManager()
    {
        var settings = Substitute.For<ISettings>();
        var settingsSaved = Substitute.For<IObservable<ISettings>>();

        _settingsManager.CurrentSettings.Returns(settings);
        _settingsManager.SettingsFileName.Returns("someFile");
        _settingsManager.SettingsSaved.Returns(settingsSaved);

        _manager.CurrentSettings.Should().Be(settings);
        _manager.SettingsFileName.Should().Be("someFile");
        _manager.SettingsSaved.Should().Be(settingsSaved);
    }

    [Fact]
    public async Task UpgradeSettingsAsync_WhenThrows_ShouldReturnFalseAndLogError()
    {
        _settingsManager.UpgradeSettingsAsync(Arg.Any<CancellationToken>())
                         .Returns(Task.FromException<bool>(new InvalidOperationException("boom")));

        var result = await _manager.UpgradeSettingsAsync(CancellationToken.None);

        result.Should().BeFalse();

        var errorCalls = _logger.ReceivedCalls()
                                .Where(c => c.GetMethodInfo().Name == nameof(_logger.Error));

        errorCalls.Count().Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task ResetSettingsAsync_ShouldLogInformationAndCallReset()
    {
        _settingsManager.ResetSettingsAsync(Arg.Any<CancellationToken>())
                        .Returns(Task.CompletedTask);

        await _manager.ResetSettingsAsync(CancellationToken.None);

        await _settingsManager.Received(1)
                              .ResetSettingsAsync(CancellationToken.None);

        var infoCalls = _logger.ReceivedCalls()
                               .Where(c => c.GetMethodInfo().Name == nameof(_logger.Information));

        infoCalls.Count().Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task SaveAsync_WhenSettingsFileNotFoundAfterSave_ShouldLogWarning()
    {
        // Arrange: ensure a path that does not exist
        var tempDir = Path.GetTempPath();
        var settingsFile = Path.Combine(tempDir, Guid.NewGuid().ToString() + ".json");

        _settingsManager.SettingsFileName.Returns(settingsFile);

        // Ensure SaveAsync succeeds
        _settingsManager.SaveAsync(Arg.Any<CancellationToken>())
                        .Returns(Task.CompletedTask);

        // Act
        await _manager.SaveAsync(CancellationToken.None);

        // Assert
        await _settingsManager.Received(1)
                              .SaveAsync(CancellationToken.None);

        var warningCalls = _logger.ReceivedCalls()
                                  .Where(c => c.GetMethodInfo().Name == nameof(_logger.Warning));

        warningCalls.Count().Should().BeGreaterThanOrEqualTo(1);
    }
}