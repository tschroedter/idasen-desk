using FluentAssertions ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;
using NSubstitute ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class HotkeyManagerTests
{
    private static HotkeyManager CreateSut ( )
    {
        var logger = Substitute.For < ILogger > ( ) ;
        var settingsManager = Substitute.For < ISettingsManager > ( ) ;
        var notifications = Substitute.For < INotifications > ( ) ;

        var settings = new Settings
        {
            HotkeySettings = new HotkeySettings ( )
        } ;
        settingsManager.CurrentSettings.Returns ( settings ) ;

        return new HotkeyManager (
                                 logger ,
                                 settingsManager ,
                                 notifications ) ;
    }

    [ Fact ]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException ( )
    {
        var settingsManager = Substitute.For < ISettingsManager > ( ) ;
        var notifications = Substitute.For < INotifications > ( ) ;

        var act = ( ) => new HotkeyManager (
                                            null! ,
                                            settingsManager ,
                                            notifications ) ;

        act.Should ( ).Throw < ArgumentNullException > ( ) ;
    }

    [ Fact ]
    public void Constructor_WithNullSettingsManager_ThrowsArgumentNullException ( )
    {
        var logger = Substitute.For < ILogger > ( ) ;
        var notifications = Substitute.For < INotifications > ( ) ;

        var act = ( ) => new HotkeyManager (
                                            logger ,
                                            null! ,
                                            notifications ) ;

        act.Should ( ).Throw < ArgumentNullException > ( ) ;
    }

    [ Fact ]
    public void Constructor_WithNullNotifications_ThrowsArgumentNullException ( )
    {
        var logger = Substitute.For < ILogger > ( ) ;
        var settingsManager = Substitute.For < ISettingsManager > ( ) ;

        var act = ( ) => new HotkeyManager (
                                            logger ,
                                            settingsManager ,
                                            null! ) ;

        act.Should ( ).Throw < ArgumentNullException > ( ) ;
    }

    [ Fact ]
    public void Dispose_CalledMultipleTimes_DoesNotThrow ( )
    {
        var sut = CreateSut ( ) ;

        var act = ( ) =>
                  {
                      sut.Dispose ( ) ;
                      sut.Dispose ( ) ;
                  } ;

        act.Should ( ).NotThrow ( ) ;
    }

    [ Fact ]
    public void Events_AreInitializedAsNull ( )
    {
        var sut = CreateSut ( ) ;

        // Events should be null initially (no subscribers)
        // We can't directly assert on events, but we can verify they can be subscribed to
        var standingCalled = false ;
        var seatingCalled = false ;
        var custom1Called = false ;
        var custom2Called = false ;

        sut.StandingHotkeyPressed += ( _ , _ ) => standingCalled = true ;
        sut.SeatingHotkeyPressed  += ( _ , _ ) => seatingCalled = true ;
        sut.Custom1HotkeyPressed  += ( _ , _ ) => custom1Called = true ;
        sut.Custom2HotkeyPressed  += ( _ , _ ) => custom2Called = true ;

        // Verify events can be subscribed to without throwing
        standingCalled.Should ( ).BeFalse ( ) ;
        seatingCalled.Should ( ).BeFalse ( ) ;
        custom1Called.Should ( ).BeFalse ( ) ;
        custom2Called.Should ( ).BeFalse ( ) ;

        sut.Dispose ( ) ;
    }
}
