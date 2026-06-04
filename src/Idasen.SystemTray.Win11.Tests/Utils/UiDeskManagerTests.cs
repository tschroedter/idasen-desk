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
        out IDeskProvider    deskProvider )
    {
        var logger = Substitute.For < ILogger > ( ) ;
        settingsManager = Substitute.For < ISettingsManager > ( ) ;
        var iconProvider    = Substitute.For < ITaskbarIconProvider > ( ) ;
        var notifications   = Substitute.For < INotifications > ( ) ;
        var scheduler       = Scheduler.Immediate ;
        var dp              = Substitute.For < IDeskProvider > ( ) ;
        var providerFactory = new Func < IDeskProvider > ( ( ) => dp ) ;
        var errorManager    = Substitute.For < IErrorManager > ( ) ;
        var settingsChanges = Substitute.For < IObserveSettingsChanges > ( ) ;
        var reconnectStrategy = new ExponentialBackoffReconnectStrategy ( logger ) ;
        var hotkeyManager   = Substitute.For < IHotkeyManager > ( ) ;

        deskProvider = dp ;

        var settings = new Settings { HeightSettings = new HeightSettings ( ) } ;
        settingsManager.CurrentSettings.Returns ( settings ) ;

        var sut = new UiDeskManager (
                                     logger ,
                                     settingsManager ,
                                     iconProvider ,
                                     notifications ,
                                     scheduler ,
                                     providerFactory ,
                                     errorManager ,
                                     settingsChanges ,
                                     reconnectStrategy ,
                                     hotkeyManager ) ;

        desk = Substitute.For < IDesk > ( ) ;
        typeof ( UiDeskManager )
           .GetField ( "_desk" ,
                       BindingFlags.Instance | BindingFlags.NonPublic )!
           .SetValue ( sut ,
                       desk ) ;

        return sut ;
    }

    [ Fact ]
    public async Task StandAsync_MovesDeskToConfiguredStandingHeight ( )
    {
        var sut = CreateSut ( out var desk ,
                              out var settingsManager ,
                              out _ ) ;
        var settings = ( Settings )settingsManager.CurrentSettings ;
        settings.HeightSettings.StandingHeightInCm = 120 ;
        settingsManager.LoadAsync ( Arg.Any < CancellationToken > ( ) ).Returns ( new ValueTask ( ) ) ;

        await sut.StandAsync ( ) ;

        await settingsManager.Received ( 1 ).LoadAsync ( Arg.Any < CancellationToken > ( ) ) ;
        desk.Received ( 1 ).MoveTo ( 120u * 100u ) ;
    }

    [ Fact ]
    public async Task SitAsync_MovesDeskToConfiguredSeatingHeight ( )
    {
        var sut = CreateSut ( out var desk ,
                              out var settingsManager ,
                              out _ ) ;
        var settings = ( Settings )settingsManager.CurrentSettings ;
        settings.HeightSettings.SeatingHeightInCm = 70 ;
        settingsManager.LoadAsync ( Arg.Any < CancellationToken > ( ) ).Returns ( new ValueTask ( ) ) ;

        await sut.SitAsync ( ) ;

        await settingsManager.Received ( 1 ).LoadAsync ( Arg.Any < CancellationToken > ( ) ) ;
        desk.Received ( 1 ).MoveTo ( 70u * 100u ) ;
    }

    [ Fact ]
    public async Task StopAsync_WhenConnected_CallsMoveStop ( )
    {
        var sut = CreateSut ( out var desk ,
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
                              out _ ) ;
        typeof ( UiDeskManager )
           .GetField ( "_desk" ,
                       BindingFlags.Instance | BindingFlags.NonPublic )!
           .SetValue ( sut ,
                       null ) ;

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
                              out _ ) ;

        await sut.MoveLockAsync ( ) ;

        await desk.Received ( 1 ).MoveLockAsync ( ) ;
    }

    [ Fact ]
    public async Task MoveUnlockAsync_WhenConnected_CallsMoveUnlock ( )
    {
        var sut = CreateSut ( out var desk ,
                              out _ ,
                              out _ ) ;

        await sut.MoveUnlockAsync ( ) ;

        await desk.Received ( 1 ).MoveUnlockAsync ( ) ;
    }

    [ Fact ]
    public async Task DisconnectAsync_WhenConnected_DisposesAndClearsDeskAndProvider ( )
    {
        var sut = CreateSut ( out var desk ,
                              out _ ,
                              out var provider ) ;
        // Assign provider field
        typeof ( UiDeskManager )
           .GetField ( "_deskProvider" ,
                       BindingFlags.Instance | BindingFlags.NonPublic )!
           .SetValue ( sut ,
                       provider ) ;

        await sut.DisconnectAsync ( ) ;

        // desk and provider should be disposed
        desk.Received ( 1 ).Dispose ( ) ;
        provider.Received ( 1 ).Dispose ( ) ;

        // and cleared from fields
        var deskField = typeof ( UiDeskManager ).GetField ( "_desk" ,
                                                            BindingFlags.Instance | BindingFlags.NonPublic )! ;
        var providerField = typeof ( UiDeskManager ).GetField ( "_deskProvider" ,
                                                                BindingFlags.Instance | BindingFlags.NonPublic )! ;
        deskField.GetValue ( sut ).Should ( ).BeNull ( ) ;
        providerField.GetValue ( sut ).Should ( ).BeNull ( ) ;
    }

    [ Fact ]
    public async Task Custom1Async_MovesDeskToConfiguredCustom1Height ( )
    {
        var sut = CreateSut ( out var desk ,
                              out var settingsManager ,
                              out _ ) ;
        var settings = ( Settings )settingsManager.CurrentSettings ;
        settings.HeightSettings.Custom1HeightInCm = 111 ;
        settingsManager.LoadAsync ( Arg.Any < CancellationToken > ( ) ).Returns ( new ValueTask ( ) ) ;

        await sut.Custom1Async ( ) ;

        await settingsManager.Received ( 1 ).LoadAsync ( Arg.Any < CancellationToken > ( ) ) ;
        desk.Received ( 1 ).MoveTo ( 111u * 100u ) ;
    }

    [ Fact ]
    public async Task Custom2Async_MovesDeskToConfiguredCustom2Height ( )
    {
        var sut = CreateSut ( out var desk ,
                              out var settingsManager ,
                              out _ ) ;
        var settings = ( Settings )settingsManager.CurrentSettings ;
        settings.HeightSettings.Custom2HeightInCm = 66 ;
        settingsManager.LoadAsync ( Arg.Any < CancellationToken > ( ) ).Returns ( new ValueTask ( ) ) ;

        await sut.Custom2Async ( ) ;

        await settingsManager.Received ( 1 ).LoadAsync ( Arg.Any < CancellationToken > ( ) ) ;
        desk.Received ( 1 ).MoveTo ( 66u * 100u ) ;
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
