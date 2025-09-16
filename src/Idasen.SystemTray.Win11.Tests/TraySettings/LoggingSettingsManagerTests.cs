using FluentAssertions ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.TraySettings ;
using NSubstitute ;
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
}