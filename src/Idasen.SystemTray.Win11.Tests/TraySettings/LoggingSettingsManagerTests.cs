using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.TraySettings ;
using NSubstitute ;
using Serilog ;
using FluentAssertions ;

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
        _manager         = new LoggingSettingsManager ( _logger ,
                                                        _settingsManager ) ;
    }

    [ Fact ]
    public async Task SaveAsync_ShouldLogDebugAndCallSaveAsync ( )
    {
        await _manager.SaveAsync ( ) ;

        _logger.Received ( 1 )
               .Debug ( Arg.Any < string > ( ) ) ;

        await _settingsManager.Received ( 1 )
                              .SaveAsync ( ) ;
    }

    [ Fact ]
    public async Task LoadAsync_ShouldLogDebugAndCallLoadAsync ( )
    {
        await _manager.LoadAsync ( ) ;

        _logger.Received ( 2 )
               .Debug ( Arg.Any < string > ( ) ) ;

        await _settingsManager.Received ( 1 )
                              .LoadAsync ( ) ;
    }

    [ Fact ]
    public async Task UpgradeSettingsAsync_ShouldLogDebugAndReturnSuccess ( )
    {
        _settingsManager.UpgradeSettingsAsync ( ).Returns ( true ) ;

        var result = await _manager.UpgradeSettingsAsync ( ) ;

        result.Should ( )
              .BeTrue ( ) ;

        _logger.Received ( 2 )
               .Debug ( Arg.Any < string > ( ) ) ;
    }

    [ Fact ]
    public async Task UpgradeSettingsAsync_ShouldLogErrorAndReturnFalseOnFailure ( )
    {
        _settingsManager.UpgradeSettingsAsync ( ).Returns ( false ) ;

        var result = await _manager.UpgradeSettingsAsync ( ) ;

        result.Should ( ).BeFalse ( ) ;

        _logger.Received ( 1 )
               .Error ( Arg.Any < string > ( ) ) ;
    }

    [ Fact ]
    public async Task SetLastKnownDeskHeight_ShouldLogDebugAndCallSetLastKnownDeskHeight ( )
    {
        const uint heightInCm = 100 ;

        await _manager.SetLastKnownDeskHeight ( heightInCm ) ;

        _logger.Received ( 1 )
               .Debug ( Arg.Any < string > ( ) ) ;

        await _settingsManager.Received ( 1 )
                              .SetLastKnownDeskHeight ( heightInCm ) ;
    }
}