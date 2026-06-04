using System.Reactive.Concurrency ;
using System.Reflection ;
using System.Windows.Input ;
using FluentAssertions ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;
using Idasen.SystemTray.Win11.Utils.Bluetooth ;
using NSubstitute ;
using Serilog ;

#pragma warning disable CA2012 // Use ValueTasks correctly - disabled for test mocking

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class UiDeskManagerTests
{
    private static UiDeskManager CreateSut (
        out IDesk            desk ,
        out ISettingsManager settingsManager ,
        out IDeskProvider    deskProvider ,
        out IDeskMovementManager deskMovementManager ,
        out IDeskConnectionManager deskConnectionManager )
    {
        var logger = Substitute.For < ILogger > ( ) ;
        settingsManager = Substitute.For < ISettingsManager > ( ) ;
        var iconProvider    = Substitute.For < ITaskbarIconProvider > ( ) ;
        var notifications   = Substitute.For < INotifications > ( ) ;
        var scheduler       = Scheduler.Immediate ;
        var dp              = Substitute.For < IDeskProvider > ( ) ;
        var errorManager    = Substitute.For < IErrorManager > ( ) ;
        var settingsChanges = Substitute.For < IObserveSettingsChanges > ( ) ;
        var hotkeyManager   = Substitute.For < IHotkeyManager > ( ) ;
        var statusBarManager = Substitute.For < IStatusBarManager > ( ) ;
        deskMovementManager = Substitute.For < IDeskMovementManager > ( ) ;
        deskConnectionManager = Substitute.For < IDeskConnectionManager > ( ) ;

        deskProvider = dp ;
        desk = Substitute.For < IDesk > ( ) ;

        var settings = new Settings { HeightSettings = new HeightSettings ( ) } ;
        settingsManager.CurrentSettings.Returns ( settings ) ;

        deskMovementManager.IsDeskAvailable ( ).Returns ( true ) ;
        deskConnectionManager.IsConnected.Returns ( true ) ;
        deskConnectionManager.CurrentDesk.Returns ( desk ) ;

        var sut = new UiDeskManager (
                                     logger ,
                                     settingsManager ,
                                     iconProvider ,
                                     notifications ,
                                     scheduler ,
                                     errorManager ,
                                     settingsChanges ,
                                     hotkeyManager ,
                                     statusBarManager ,
                                     deskMovementManager ,
                                     deskConnectionManager ) ;

        return sut ;
    }

    [ Fact ]
    public async Task StandAsync_MovesDeskToConfiguredStandingHeight ( )
    {
        var sut = CreateSut ( out _ ,
                              out var settingsManager ,
                              out _ ,
                              out var deskMovementManager ,
                              out _ ) ;
        var settings = ( Settings )settingsManager.CurrentSettings ;
        settings.HeightSettings.StandingHeightInCm = 120 ;

        await sut.StandAsync ( ) ;

        await deskMovementManager.Received ( 1 ).MoveToHeightAsync ( 120u , nameof ( UiDeskManager.StandAsync ) ) ;
    }

    [ Fact ]
    public async Task SitAsync_MovesDeskToConfiguredSeatingHeight ( )
    {
        var sut = CreateSut ( out _ ,
                              out var settingsManager ,
                              out _ ,
                              out var deskMovementManager ,
                              out _ ) ;
        var settings = ( Settings )settingsManager.CurrentSettings ;
        settings.HeightSettings.SeatingHeightInCm = 70 ;

        await sut.SitAsync ( ) ;

        await deskMovementManager.Received ( 1 ).MoveToHeightAsync ( 70u , nameof ( UiDeskManager.SitAsync ) ) ;
    }

    [ Fact ]
    public async Task StopAsync_WhenConnected_CallsMoveStop ( )
    {
        var sut = CreateSut ( out var desk ,
                              out _ ,
                              out _ ,
                              out _ ,
                              out _ ) ;

        await sut.StopAsync ( ) ;

        await desk.Received ( 1 ).MoveStopAsync ( ) ;
    }

    [ Fact ]
    public async Task StopAsync_WhenNotConnected_DoesNothing ( )
    {
        var sut = CreateSut ( out var desk ,
                              out _ ,
                              out _ ,
                              out _ ,
                              out var deskConnectionManager ) ;
        deskConnectionManager.IsConnected.Returns ( false ) ;

        await sut.StopAsync ( ) ;

        desk.DidNotReceive ( ).MoveTo ( Arg.Any < uint > ( ) ) ;

        await desk.DidNotReceive ( ).MoveStopAsync ( ) ;
        await desk.DidNotReceive ( ).MoveStopAsync ( ) ;
        await desk.DidNotReceive ( ).MoveUnlockAsync ( ) ;
    }

    [ Fact ]
    public async Task MoveLockAsync_WhenConnected_CallsMoveLock ( )
    {
        var sut = CreateSut ( out var desk ,
                              out _ ,
                              out _ ,
                              out _ ,
                              out _ ) ;

        await sut.MoveLockAsync ( ) ;

        await desk.Received ( 1 ).MoveLockAsync ( ) ;
    }

    [ Fact ]
    public async Task MoveUnlockAsync_WhenConnected_CallsMoveUnlock ( )
    {
        var sut = CreateSut ( out var desk ,
                              out _ ,
                              out _ ,
                              out _ ,
                              out _ ) ;

        await sut.MoveUnlockAsync ( ) ;

        await desk.Received ( 1 ).MoveUnlockAsync ( ) ;
    }

    [ Fact ]
    public async Task DisconnectAsync_WhenConnected_DisposesAndClearsDeskAndProvider ( )
    {
        var sut = CreateSut ( out _ ,
                              out _ ,
                              out _ ,
                              out _ ,
                              out var deskConnectionManager ) ;

        await sut.DisconnectAsync ( ) ;

        await deskConnectionManager.Received ( 1 ).DisconnectAsync ( ) ;
    }

    [ Fact ]
    public async Task Custom1Async_MovesDeskToConfiguredCustom1Height ( )
    {
        var sut = CreateSut ( out _ ,
                              out var settingsManager ,
                              out _ ,
                              out var deskMovementManager ,
                              out _ ) ;
        var settings = ( Settings )settingsManager.CurrentSettings ;
        settings.HeightSettings.Custom1HeightInCm = 111 ;

        await sut.Custom1Async ( ) ;

        await deskMovementManager.Received ( 1 ).MoveToHeightAsync ( 111u , nameof ( UiDeskManager.Custom1Async ) ) ;
    }

    [ Fact ]
    public async Task Custom2Async_MovesDeskToConfiguredCustom2Height ( )
    {
        var sut = CreateSut ( out _ ,
                              out var settingsManager ,
                              out _ ,
                              out var deskMovementManager ,
                              out _ ) ;
        var settings = ( Settings )settingsManager.CurrentSettings ;
        settings.HeightSettings.Custom2HeightInCm = 66 ;

        await sut.Custom2Async ( ) ;

        await deskMovementManager.Received ( 1 ).MoveToHeightAsync ( 66u , nameof ( UiDeskManager.Custom2Async ) ) ;
    }

    // Hotkey Configuration Tests

    [ Fact ]
    public void HotkeySettings_DefaultConfiguration_EnablesHotkeys ( )
    {
        // Arrange & Act
        var settings = new Settings ( ) ;

        // Assert
        settings.HotkeySettings.GlobalHotkeysEnabled.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void HotkeySettings_CanBeDisabled_ViaConfiguration ( )
    {
        // Arrange
        var settings = new Settings
        {
            HotkeySettings = new HotkeySettings
            {
                GlobalHotkeysEnabled = false
            }
        } ;

        // Act & Assert
        settings.HotkeySettings.GlobalHotkeysEnabled.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public void HotkeySettings_CustomKeys_CanBeConfigured ( )
    {
        // Arrange
        var settings = new Settings
        {
            HotkeySettings = new HotkeySettings
            {
                StandingKey       = "F9" ,
                StandingModifiers = "Control, Alt" ,
                SeatingKey        = "F10" ,
                SeatingModifiers  = "Control, Alt"
            }
        } ;

        // Act & Assert
        using var scope = new FluentAssertions.Execution.AssertionScope ( ) ;
        settings.HotkeySettings.StandingKey.Should ( ).Be ( "F9" ) ;
        settings.HotkeySettings.StandingModifiers.Should ( ).Be ( "Control, Alt" ) ;
        settings.HotkeySettings.SeatingKey.Should ( ).Be ( "F10" ) ;
        settings.HotkeySettings.SeatingModifiers.Should ( ).Be ( "Control, Alt" ) ;
    }

    [ Fact ]
    public void HotkeySettings_DefaultsToEnabled ( )
    {
        // Arrange & Act
        var settings = new HotkeySettings ( ) ;

        // Assert
        settings.GlobalHotkeysEnabled.Should ( ).BeTrue ( ) ;
    }
}
